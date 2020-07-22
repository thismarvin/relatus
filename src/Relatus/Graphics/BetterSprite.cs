using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class BetterSprite
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public RectangleF SampleRegion { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly Texture2D texture;
        private float texelWidth;
        private float texelHeight;

        private bool modelChanged;
        private DynamicVertexBuffer modelBuffer;
        private VertexBufferBinding[] vertexBufferBindings;
        private VertexBuffer test;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly IndexBuffer indexBuffer;
        private static readonly Effect spriteShader;

        static BetterSprite()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), 6, BufferUsage.WriteOnly);
            indexBuffer.SetData
            (
                new short[]
                {
                    0, 1, 2,
                    0, 2, 3
                }
            );

            spriteShader = AssetManager.GetEffect("Relatus_SpriteShader");
        }

        public BetterSprite(Texture2D texture)
        {
            this.texture = texture;
            texelWidth = 1f / texture.Width;
            texelHeight = 1f / texture.Height;

            SampleRegion = new RectangleF(110, 0, 70, 113);

            Width = texture.Width;
            Height = texture.Height;
            ApplyChanges();
        }

        public BetterSprite SetPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            modelChanged = true;

            return this;
        }

        public BetterSprite SetSampleRegion(int x, int y, int width, int height)
        {
            SampleRegion = new RectangleF(x, y, width, height);

            modelChanged = true;

            return this;
        }

        public BetterSprite ApplyChanges()
        {         
            Vector2 topLeft = new Vector2((float)MathExt.RemapRange(SampleRegion.X, 0, texture.Width, 0, 1), (float)MathExt.RemapRange(SampleRegion.Y, 0, texture.Height, 0, 1));           
            Vector2 topRight = topLeft + new Vector2(texelWidth * SampleRegion.Width, 0);
            Vector2 bottomRight = topLeft + new Vector2(texelWidth * SampleRegion.Width, texelHeight * SampleRegion.Height);
            Vector2 bottomLeft = topLeft + new Vector2(0, texelHeight * SampleRegion.Height);

            test = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
            test.SetData(new VertexPositionColorTexture[] {
                new VertexPositionColorTexture(new Vector3(0, 0, 0), Color.White, topLeft),
                new VertexPositionColorTexture(new Vector3(0, -1, 0), Color.White, bottomLeft),
                new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, bottomRight),
                new VertexPositionColorTexture(new Vector3(1, 0, 0), Color.White, topRight),
            });

            modelBuffer?.Dispose();
            modelBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
            modelBuffer.SetData(new VertexTransform[] { GetVertexTransformColor() });

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(test),
                new VertexBufferBinding(modelBuffer, 0, 1)
            };

            return this;
        }

        internal VertexTransform GetVertexTransformColor()
        {
            Vector3 scale = new Vector3(SampleRegion.Width, SampleRegion.Height, 1);
            Vector3 translation = new Vector3(X, Y, 0);

            return new VertexTransform(scale, Vector2.Zero, 0, translation);
        }

        public void Draw(Camera camera)
        {
            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = indexBuffer;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.Textures[0] = texture;

            spriteShader.Parameters["SpriteTexture"].SetValue(texture);
            spriteShader.Parameters["WorldViewProjection"].SetValue(camera.WVP);

            foreach (EffectPass pass in spriteShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, 1);
            }

        }
    }
}
