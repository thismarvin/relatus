using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    public class SpriteGroup : DrawGroup<BetterSprite>, IDisposable
    {
        private readonly RenderOptions sharedRenderOptions;
        private readonly Texture2D sharedTexture;

        private readonly VertexPosition[] vertexPositions;
        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;
        private readonly VertexTexture[] textureCoords;
        private bool dataChanged;
        
        private readonly short[] indices;
        private readonly IndexBuffer indexBuffer;

        private DynamicVertexBuffer vertexPositionBuffer;
        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private DynamicVertexBuffer textureCoordBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private int totalPrimitives;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly VertexPosition[] sharedGeometry;
        private static readonly Effect spriteShader;
        private static readonly EffectPass spritePass;

        static SpriteGroup()
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

        public SpriteGroup(RenderOptions sharedRenderOptions, Texture2D sharedTexture, int capacity) : base(capacity)
        {
            this.sharedRenderOptions = sharedRenderOptions;
            this.sharedTexture = sharedTexture;

            vertexPositions = new VertexPosition[capacity * 4];
            transforms = new VertexTransform[capacity * 4];
            colors = new VertexColor[capacity * 4];
            textureCoords = new VertexTexture[capacity * 4];

            indices = new short[capacity * 6];
            for (int i = 0; i < capacity; i++)
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

        protected override bool ConditionToAdd(BetterSprite entry)
        {
            if (entry == null)
                return false;

            return entry.RenderOptions.Equals(sharedRenderOptions) && entry.Texture == sharedTexture;
        }

        public override bool Add(BetterSprite entry)
        {
            if (totalPrimitives >= capacity)
                return false;

            if (ConditionToAdd(entry))
            {
                VertexTransform transform = entry.GetVertexTransform();
                VertexColor color = entry.GetVertexColor();
                VertexTexture[] textureCoords = entry.GetTextureCoords();

                vertexPositions[groupIndex + 0] = sharedGeometry[0]; 
                vertexPositions[groupIndex + 1] = sharedGeometry[1]; 
                vertexPositions[groupIndex + 2] = sharedGeometry[2];
                vertexPositions[groupIndex + 3] = sharedGeometry[3]; 

                transforms[groupIndex + 0] = transform;
                transforms[groupIndex + 1] = transform;
                transforms[groupIndex + 2] = transform;
                transforms[groupIndex + 3] = transform;

                colors[groupIndex + 0] = color;
                colors[groupIndex + 1] = color;
                colors[groupIndex + 2] = color;
                colors[groupIndex + 3] = color;

                this.textureCoords[groupIndex + 0] = textureCoords[0];
                this.textureCoords[groupIndex + 1] = textureCoords[1];
                this.textureCoords[groupIndex + 2] = textureCoords[2];
                this.textureCoords[groupIndex + 3] = textureCoords[3];

                groupIndex += 4;
                dataChanged = true;

                totalPrimitives++;

                return true;
            }

            return false;
        }

        private void UpdateBuffer()
        {
            vertexPositionBuffer?.Dispose();
            vertexPositionBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPosition), vertexPositions.Length, BufferUsage.WriteOnly);
            vertexPositionBuffer.SetData(vertexPositions);

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), colors.Length, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors);

            textureCoordBuffer?.Dispose();
            textureCoordBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTexture), textureCoords.Length, BufferUsage.WriteOnly);
            textureCoordBuffer.SetData(textureCoords);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(vertexPositionBuffer),
                new VertexBufferBinding(transformBuffer),
                new VertexBufferBinding(colorBuffer),
                new VertexBufferBinding(textureCoordBuffer)
            };
        }

        public override void Draw(Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

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
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, totalPrimitives * 2);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.Textures[0] = sharedTexture;
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, totalPrimitives * 2);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    vertexPositionBuffer.Dispose();
                    transformBuffer.Dispose();
                    colorBuffer.Dispose();
                    textureCoordBuffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SpriteGroup()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
