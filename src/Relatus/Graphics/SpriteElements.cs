using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    internal class SpriteElements : SpriteGroup
    {
        public const uint MaxBatchSize = short.MaxValue / 6;

        private readonly RenderOptions sharedRenderOptions;
        private readonly Texture2D sharedTexture;

        private readonly VertexPosition[] vertexPositions;
        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;
        private readonly VertexTexture[] textureCoords;
        private bool dataModified;

        private readonly short[] indices;
        private readonly IndexBuffer indexBuffer;

        private DynamicVertexBuffer vertexPositionBuffer;
        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private DynamicVertexBuffer textureCoordBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private uint index;
        private uint count;
        private uint totalPrimitives;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly VertexPosition[] sharedGeometry;
        private static readonly Effect spriteShader;
        private static readonly EffectPass spritePass;

        static SpriteElements()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            spriteShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            spritePass = spriteShader.Techniques[2].Passes[0];

            sharedGeometry = new VertexPosition[]
            {
                new VertexPosition(new Vector3(0, 0, 0)),
                new VertexPosition(new Vector3(0, -1, 0)),
                new VertexPosition(new Vector3(1, -1, 0)),
                new VertexPosition(new Vector3(1, 0, 0)),
            };
        }

        public SpriteElements(uint batchSize, Texture2D sharedTexture, RenderOptions sharedRenderOptions) : base(BatchExecution.DrawElements, batchSize)
        {
            if (BatchSize > MaxBatchSize)
                throw new RelatusException($"SpriteElements does not support support a batch size greater than {MaxBatchSize}.", new ArgumentOutOfRangeException());

            this.sharedTexture = sharedTexture;
            this.sharedRenderOptions = sharedRenderOptions;

            vertexPositions = new VertexPosition[batchSize * 4];
            transforms = new VertexTransform[batchSize * 4];
            colors = new VertexColor[batchSize * 4];
            textureCoords = new VertexTexture[batchSize * 4];

            indices = new short[batchSize * 6];
            for (int i = 0; i < batchSize; i++)
            {
                int start = 6 * i;
                int buffer = 4 * i;
                indices[start + 0] = (short)(buffer + 0);
                indices[start + 1] = (short)(buffer + 1);
                indices[start + 2] = (short)(buffer + 2);
                indices[start + 3] = (short)(buffer + 0);
                indices[start + 4] = (short)(buffer + 2);
                indices[start + 5] = (short)(buffer + 3);
            }

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public override bool Add(Sprite sprite)
        {
            if (count >= BatchSize)
                return false;

            if (!sprite.Texture.Equals(sharedTexture) || !sprite.RenderOptions.Equals(sharedRenderOptions))
                return false;

            VertexTransform transform = sprite.GetVertexTransform();
            VertexColor color = sprite.GetVertexColor();
            VertexTexture[] textureCoords = sprite.GetTextureCoords();

            vertexPositions[index + 0] = sharedGeometry[0];
            vertexPositions[index + 1] = sharedGeometry[1];
            vertexPositions[index + 2] = sharedGeometry[2];
            vertexPositions[index + 3] = sharedGeometry[3];

            transforms[index + 0] = transform;
            transforms[index + 1] = transform;
            transforms[index + 2] = transform;
            transforms[index + 3] = transform;

            colors[index + 0] = color;
            colors[index + 1] = color;
            colors[index + 2] = color;
            colors[index + 3] = color;

            this.textureCoords[index + 0] = textureCoords[0];
            this.textureCoords[index + 1] = textureCoords[1];
            this.textureCoords[index + 2] = textureCoords[2];
            this.textureCoords[index + 3] = textureCoords[3];

            dataModified = true;

            index += 4;
            count++;
            totalPrimitives += 2;

            return true;

        }

        public override DrawGroup<Sprite> ApplyChanges()
        {
            if (!dataModified)
                return this;

            int vertexCount = (int)count * 4;

            vertexPositionBuffer?.Dispose();
            vertexPositionBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPosition), vertexCount, BufferUsage.WriteOnly);
            vertexPositionBuffer.SetData(vertexPositions, 0, vertexCount);

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), vertexCount, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms, 0, vertexCount);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), vertexCount, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors, 0, vertexCount);

            textureCoordBuffer?.Dispose();
            textureCoordBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTexture), vertexCount, BufferUsage.WriteOnly);
            textureCoordBuffer.SetData(textureCoords, 0, vertexCount);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(vertexPositionBuffer),
                new VertexBufferBinding(transformBuffer),
                new VertexBufferBinding(colorBuffer),
                new VertexBufferBinding(textureCoordBuffer)
            };

            dataModified = false;

            return this;
        }

        public override DrawGroup<Sprite> Draw(Camera camera)
        {
            if (dataModified)
                throw new RelatusException("The sprite group was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = sharedRenderOptions.SamplerState;
            graphicsDevice.BlendState = sharedRenderOptions.BlendState;
            graphicsDevice.DepthStencilState = sharedRenderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = indexBuffer;

            spriteShader.Parameters["WVP"].SetValue(camera.WVP);
            spriteShader.Parameters["SpriteTexture"].SetValue(sharedTexture);

            spritePass.Apply();

            if (sharedRenderOptions.Effect == null)
            {
                graphicsDevice.Textures[0] = sharedTexture;
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)totalPrimitives);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.Textures[0] = sharedTexture;
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)totalPrimitives);
                }
            }

            return this;
        }

        protected override void OnDispose()
        {
            vertexPositionBuffer.Dispose();
            transformBuffer.Dispose();
            colorBuffer.Dispose();
            textureCoordBuffer.Dispose();
            indexBuffer.Dispose();
        }
    }
}
