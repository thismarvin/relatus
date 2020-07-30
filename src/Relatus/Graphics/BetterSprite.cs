using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class BetterSprite
    {
        public Texture2D Texture
        {
            get => texture;
            set => SetTexture(value);
        }
        public float X
        {
            get => x;
            set => SetPosition(value, y, z);
        }
        public float Y
        {
            get => y;
            set => SetPosition(x, value, z);
        }
        public float Z
        {
            get => z;
            set => SetPosition(x, y, value);
        }
        public Vector3 Translation
        {
            get => translation;
            set => SetTranslation(value.X, value.Y, value.Z);
        }
        public Vector3 Scale
        {
            get => scale;
            set => SetScale(value.X, value.Y, value.Z);
        }
        public Vector2 RotationOffset
        {
            get => rotationOffset;
            set => SetRotationOffset(value.X, value.Y);
        }
        public float Rotation
        {
            get => rotation;
            set => SetRotation(value);
        }
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }
        public RectangleF SampleRegion
        {
            get => sampleRegion;
            set => SetSampleRegion((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private Texture2D texture;
        private float texelWidth;
        private float texelHeight;

        private float x;
        private float y;
        private float z;
        private Vector3 translation;
        private Vector3 scale;
        private Vector2 rotationOffset;
        private float rotation;
        private Color tint;
        private RectangleF sampleRegion;

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

        public static BetterSprite Create()
        {
            return new BetterSprite();
        }

        public BetterSprite()
        {
            Tint = Color.White;
            Scale = Vector3.One;

            modelChanged = true;
            textureChanged = true;
        }

        public BetterSprite SetTexture(Texture2D texture)
        {
            this.texture = texture;

            texelWidth = 1f / texture.Width;
            texelHeight = 1f / texture.Height;

            textureChanged = true;

            return this;
        }

        public BetterSprite SetPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            modelChanged = true;

            return this;
        }

        public BetterSprite SetTranslation(float x, float y, float z)
        {
            translation = new Vector3(x, y, z);

            modelChanged = true;

            return this;
        }

        public BetterSprite SetScale(float x, float y, float z)
        {
            scale = new Vector3(x, y, z);

            modelChanged = true;

            return this;
        }

        public BetterSprite SetRotationOffset(float x, float y)
        {
            rotationOffset = new Vector2(x, y);

            modelChanged = true;

            return this;
        }

        public BetterSprite SetRotation(float rotation)
        {
            this.rotation = rotation;

            modelChanged = true;

            return this;
        }

        public BetterSprite SetTint(Color tint)
        {
            this.tint = tint;

            textureChanged = true;

            return this;
        }

        public BetterSprite SetSampleRegion(int x, int y, int width, int height)
        {
            sampleRegion = new RectangleF(x, y, width, height);

            Width = width;
            Height = height;

            textureChanged = true;
            modelChanged = true;

            return this;
        }

        public BetterSprite SetSampleRegion(ImageRegion region)
        {
            return SetSampleRegion(region.X, region.Y, region.Width, region.Height);
        }

        public BetterSprite ApplyChanges()
        {
            if (textureChanged)
            {
                Vector2 topLeft = new Vector2((float)MathExt.RemapRange(SampleRegion.X, 0, texture.Width, 0, 1), (float)MathExt.RemapRange(SampleRegion.Y, 0, texture.Height, 0, 1));
                Vector2 topRight = topLeft + new Vector2(texelWidth * SampleRegion.Width, 0);
                Vector2 bottomRight = topLeft + new Vector2(texelWidth * SampleRegion.Width, texelHeight * SampleRegion.Height);
                Vector2 bottomLeft = topLeft + new Vector2(0, texelHeight * SampleRegion.Height);

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
            Vector3 scale = new Vector3(SampleRegion.Width * Scale.X, SampleRegion.Height * Scale.Y, Scale.Z);
            Vector3 translation = new Vector3(X + Translation.X, Y + Translation.Y, Z + Translation.Z);

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
