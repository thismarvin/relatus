using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public float Z
        {
            get => z;
            set
            {
                z = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public float Width
        {
            get => width;
            set
            {
                width = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public float Height
        {
            get => height;
            set
            {
                height = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public virtual float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public Vector2 RotationOffset
        {
            get => rotationOffset;
            set
            {
                rotationOffset = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public Vector3 Translation
        {
            get => translation;
            set
            {
                translation = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }
        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                modelChanged = true;
                transformNeedsUpdating = true;
            }
        }

        public GeometryData Geometry
        {
            get => geometry;
            set
            {
                AttachGeometry(value);
            }
        }
        #endregion

        public Vector3 Position { get => new Vector3(X, Y, Z); }
        public Vector3 Center { get => new Vector3(X + Width / 2, Y + Height / 2, Z); }
        public RectangleF Bounds { get => new RectangleF(X, Y, Width, Height); }

        private float x;
        private float y;
        private float z;
        private float width;
        private float height;
        private Color color;

        private float rotation;
        private Vector2 rotationOffset;
        private Vector3 translation;
        private Vector3 scale;

        private GeometryData geometry;

        private bool geometryChanged;
        private bool modelChanged;
        protected DynamicVertexBuffer modelBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;

        private bool transformNeedsUpdating;
        private Matrix transformCache;

        private static readonly GraphicsDevice graphicsDevice;
        static Polygon()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        public Polygon()
        {
            color = Color.White;
            scale = new Vector3(1);

            transformNeedsUpdating = true;
            transformCache = Matrix.Identity;
        }

        public Polygon AttachGeometry(GeometryData geometry)
        {
            if (this.geometry != null && !this.geometry.Managed)
            {
                this.geometry.Dispose();
            }

            this.geometry = geometry;
            geometryChanged = true;

            return this;
        }

        public Polygon ApplyChanges()
        {
            if (!geometryChanged && !modelChanged)
                return this;

            if (modelChanged)
            {
                modelBuffer?.Dispose();
                modelBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), 1, BufferUsage.WriteOnly);
                modelBuffer.SetData(new VertexTransformColor[] { GetVertexTransformColor() });
            }

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(Geometry.VertexBuffer),
                new VertexBufferBinding(modelBuffer, 0, 1)
            };

            geometryChanged = false;
            modelChanged = false;

            return this;
        }

        public virtual Polygon SetPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetCenter(float x, float y)
        {
            this.x = x - Width / 2;
            this.y = y + Height / 2;

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetBounds(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public Matrix CalculateTransform()
        {
            if (transformNeedsUpdating)
            {
                transformNeedsUpdating = false;

                transformCache =
                Matrix.CreateScale(Width * Scale.X, Height * Scale.Y, 1 * Scale.Z) *
                Matrix.CreateTranslation(-new Vector3(RotationOffset.X, RotationOffset.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(X + Translation.X + RotationOffset.X, Y + Translation.Y + RotationOffset.Y, Translation.Z);
            }

            return transformCache;
        }

        internal VertexTransformColor GetVertexTransformColor()
        {
            Vector3 scale = new Vector3(Width * Scale.X, Height * Scale.Y, Scale.Z);
            Vector3 translation = new Vector3(X + Translation.X, Y + Translation.Y, Z + Translation.Z);            

            return new VertexTransformColor(scale, RotationOffset, rotation, translation, Color);
        }

        public virtual void Draw(Camera camera)
        {
            if (geometryChanged || modelChanged)
            {
                throw new RelatusException("The polygon was modified, but ApplyChanges() was never called.", new MethodExpectedException());
            }

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = Geometry.IndexBuffer;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, Geometry.TotalTriangles, 1);
            }
        }
    }
}
