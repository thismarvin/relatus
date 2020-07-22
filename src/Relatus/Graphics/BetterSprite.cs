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
        public float Rotation { get; set; }
        public Vector2 RotationOffset { get; set; }
        public Color Tint { get; set; }
        public RectangleF SampleRegion { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly Texture2D texture;
        private readonly float texelWidth;
        private readonly float texelHeight;

        private bool modelChanged;
        private bool textureChanged;
        private DynamicVertexBuffer modelBuffer;
        private DynamicVertexBuffer textureBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly GeometryData geometry;
        private static readonly Effect spriteShader;

        static BetterSprite()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            geometry = GeometryManager.GetShapeData(ShapeType.Square);
            spriteShader = AssetManager.GetEffect("Relatus_SpriteShader");
        }

        public BetterSprite(Texture2D texture)
        {
            this.texture = texture;
            texelWidth = 1f / texture.Width;
            texelHeight = 1f / texture.Height;

            SampleRegion = new RectangleF(110, 0, 70, 113);
            Tint = Color.White;

            Width = texture.Width;
            Height = texture.Height;

            modelChanged = true;
            textureChanged = true;

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

            if (textureChanged)
            {
                textureBuffer?.Dispose();
                textureBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColorTexture), 4, BufferUsage.WriteOnly);
                textureBuffer.SetData(new VertexColorTexture[]
                {
                    new VertexColorTexture(Tint, topLeft),
                    new VertexColorTexture(Tint, bottomLeft),
                    new VertexColorTexture(Tint, bottomRight),
                    new VertexColorTexture(Tint, topRight),
                });
            }

            if (modelChanged)
            {
                modelBuffer?.Dispose();
                modelBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
                modelBuffer.SetData(new VertexTransform[] { GetVertexTransformColor() });
            }

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(modelBuffer, 0, 1),
                new VertexBufferBinding(textureBuffer),
                new VertexBufferBinding(geometry.VertexBuffer),
            };

            textureChanged = false;
            modelChanged = false;

            return this;
        }

        internal VertexTransform GetVertexTransformColor()
        {
            Vector3 scale = new Vector3(SampleRegion.Width, SampleRegion.Height, 1);
            Vector3 translation = new Vector3(X, Y, Z);

            return new VertexTransform(scale, RotationOffset, Rotation, translation);
        }

        public void Draw(Camera camera)
        {
            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;
            graphicsDevice.Textures[0] = texture;

            spriteShader.Parameters["SpriteTexture"].SetValue(texture);
            spriteShader.Parameters["WorldViewProjection"].SetValue(camera.WVP);

            foreach (EffectPass pass in spriteShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
            }
        }
    }
}
