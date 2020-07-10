using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.ECS;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class Polygon
    {
        #region Properties
        public float X
        {
            get => x;
            set
            {
                x = value;
                transformChanged = true;
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                transformChanged = true;
            }
        }
        public float Width
        {
            get => width;
            set
            {
                width = value;
                transformChanged = true;
            }
        }
        public float Height
        {
            get => height;
            set
            {
                height = value;
                transformChanged = true;
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                transformChanged = true;
            }
        }
        public virtual float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                transformChanged = true;
            }
        }
        public Vector2 RotationOffset
        {
            get => rotationOffset;
            set
            {
                rotationOffset = value;
                transformChanged = true;
            }
        }
        public Vector3 Translation
        {
            get => translation;
            set
            {
                translation = value;
                transformChanged = true;
            }
        }
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                transformChanged = true;
            }
        }

        public GeometryData Geometry
        {
            get => geometry;
            set
            {
                if (geometry != null && !geometry.Managed)
                {
                    geometry.Dispose();
                }

                geometry = value;
            }
        }
        #endregion

        public Vector2 Position { get => new Vector2(X, Y); }
        public Vector2 Center { get => new Vector2(X + Width / 2, Y + Height / 2); }
        public RectangleF Bounds { get => new RectangleF(X, Y, Width, Height); }

        private float x;
        private float y;
        private float width;
        private float height;
        private Color color;

        private float rotation;
        private Vector2 rotationOffset;
        private Vector3 translation;
        private Vector3 scale;

        private GeometryData geometry;

        private bool transformChanged;
        protected DynamicVertexBuffer modelBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;

        private static readonly SpriteBatch spriteBatch;

        private Matrix transformCache;

        static Polygon()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public Polygon(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            color = Color.White;
            rotation = 0;
            rotationOffset = Vector2.Zero;
            translation = Vector3.Zero;
            scale = new Vector3(1);

            transformCache = Matrix.Identity;
        }

        public Polygon ApplyChanges()
        {
            transformChanged = false;
            UpdateModelBuffer();

            return this;
        }

        public virtual Polygon SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;

            transformChanged = true;

            return this;
        }

        public virtual Polygon SetCenter(float x, float y)
        {
            return SetPosition(x - Width / 2, y - Height / 2);
        }

        public virtual Polygon SetBounds(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            transformChanged = true;

            return this;
        }

        public Polygon AttachGeometry(GeometryData geometry)
        {
            this.geometry = geometry;

            return this;
        }

        public Matrix CalculateTransform()
        {
            if (transformCache == Matrix.Identity)
            {
                transformCache =
                Matrix.CreateScale(Width * Scale.X, Height * Scale.Y, 1 * Scale.Z) *

                Matrix.CreateTranslation(-new Vector3(RotationOffset.X, RotationOffset.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *

                Matrix.CreateTranslation(X + Translation.X + RotationOffset.X, Y + Translation.Y + RotationOffset.Y, Translation.Z) *

                Matrix.Identity;
            }

            return transformCache;
        }

        internal VertexTransformColor GetVertexTransformColor()
        {
            return new VertexTransformColor(new CPosition(X, Y), new CDimension(Width, Height), new CTransform(Scale, Rotation, RotationOffset, Translation), new CColor(Color));
        }

        private void UpdateModelBuffer()
        {
            modelBuffer?.Dispose();

            modelBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), 1, BufferUsage.WriteOnly);
            modelBuffer.SetData(new VertexTransformColor[] { GetVertexTransformColor() });

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(Geometry.Geometry),
                new VertexBufferBinding(modelBuffer, 0, 1)
            };
        }

        public virtual void Draw(Camera camera)
        {
            if (transformChanged)
            {
                throw new RelatusException("The polygon's transform was modified, but ApplyChanges() was never called.");
            }

            spriteBatch.GraphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            spriteBatch.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            spriteBatch.GraphicsDevice.Indices = Geometry.Indices;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, Geometry.TotalTriangles, 1);
            }
        }
    }
}
