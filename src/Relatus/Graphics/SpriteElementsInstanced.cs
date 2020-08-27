using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    internal class SpriteElementsInstanced : SpriteGroup
    {
        private readonly RenderOptions sharedRenderOptions;
        private readonly Texture2D sharedTexture;

        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;
        private readonly VertexTexture[] textureCoords;
        private bool dataModified;

        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private DynamicVertexBuffer textureCoordBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private uint textureCoordIndex;
        private uint count;
        private uint totalPrimitives;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly GeometryData sharedGeometry;
        private static readonly Effect spriteShader;
        private static readonly EffectPass spritePass;

        static SpriteElementsInstanced()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            spriteShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            spritePass = spriteShader.Techniques[2].Passes[0];

            sharedGeometry = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public SpriteElementsInstanced(uint batchSize, Texture2D sharedTexture, RenderOptions sharedRenderOptions) : base(BatchExecution.DrawElementsInstanced, batchSize)
        {
            this.sharedTexture = sharedTexture;
            this.sharedRenderOptions = sharedRenderOptions;

            transforms = new VertexTransform[batchSize];
            colors = new VertexColor[batchSize];
            textureCoords = new VertexTexture[batchSize * 4];
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

            transforms[count] = transform;
            colors[count] = color;

            this.textureCoords[textureCoordIndex + 0] = textureCoords[0];
            this.textureCoords[textureCoordIndex + 1] = textureCoords[1];
            this.textureCoords[textureCoordIndex + 2] = textureCoords[2];
            this.textureCoords[textureCoordIndex + 3] = textureCoords[3];

            dataModified = true;

            textureCoordIndex += 4;
            count++;
            totalPrimitives += 2;

            return true;
        }

        public override DrawGroup<Sprite> ApplyChanges()
        {
            if (!dataModified)
                return this;

            int vertexCount = (int)count * 4;

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), (int)count, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms, 0, (int)count);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), (int)count, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors, 0, (int)count);

            textureCoordBuffer?.Dispose();
            textureCoordBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTexture), vertexCount, BufferUsage.WriteOnly);
            textureCoordBuffer.SetData(textureCoords, 0, vertexCount);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(sharedGeometry.VertexBuffer),
                new VertexBufferBinding(transformBuffer, 0, 1),
                new VertexBufferBinding(colorBuffer, 0, 1),
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
            graphicsDevice.Indices = sharedGeometry.IndexBuffer;

            spriteShader.Parameters["WVP"].SetValue(camera.WVP);
            spriteShader.Parameters["SpriteTexture"].SetValue(sharedTexture);

            spritePass.Apply();

            if (sharedRenderOptions.Effect == null)
            {
                graphicsDevice.Textures[0] = sharedTexture;
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.TotalTriangles, (int)count);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.Textures[0] = sharedTexture;
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.TotalTriangles, (int)count);
                }
            }

            return this;
        }

        protected override void OnDispose()
        {
            transformBuffer.Dispose();
            colorBuffer.Dispose();
            textureCoordBuffer.Dispose();
        }
    }
}
