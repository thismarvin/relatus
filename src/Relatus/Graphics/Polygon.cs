using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class Polygon : IDisposable
    {
        #region Properties
        public float X
        {
            get => x;
            set
            {
                if (x == value)
                    return;

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
                if (y == value)
                    return;

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
                if (z == value)
                    return;

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
                if (width == value)
                    return;

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
                if (height == value)
                    return;

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
                if (color == value)
                    return;

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
                if (rotation == value)
                    return;

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
                if (rotationOffset == value)
                    return;

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
                if (translation == value)
                    return;

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
                if (scale == value)
                    return;

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

        public Vector3 Position
        {
            get => new Vector3(x, y, z);
            set => SetPosition(value.X, value.Y, value.Z);
        }

        // I just want to note that although this is a Vector3, it is really treated like a Vector2.
        public Vector3 Center
        {
            get => new Vector3(x + width / 2, y - height / 2, z);
            set => SetCenter(value.X, value.Y);
        }

        public RectangleF Bounds
        {
            get => new RectangleF(x, y, width, height);
            set => SetBounds(value.X, value.Y, value.Width, value.Height);
        }

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
        private bool transformNeedsUpdating;
        private Matrix transformCache;
        private DynamicVertexBuffer modelBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

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
            if (this.geometry == geometry)
                return this;

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
                new VertexBufferBinding(geometry.VertexBuffer),
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
                Matrix.CreateScale(width * scale.X, height * scale.Y, 1 * scale.Z) *
                Matrix.CreateTranslation(-new Vector3(rotationOffset.X, rotationOffset.Y, 0)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(x + translation.X + rotationOffset.X, y + translation.Y + rotationOffset.Y, translation.Z);
            }

            return transformCache;
        }

        internal VertexTransformColor GetVertexTransformColor()
        {
            Vector3 scale = new Vector3(width * this.scale.X, height * this.scale.Y, this.scale.Z);
            Vector3 translation = new Vector3(x + this.translation.X, y + this.translation.Y, z + this.translation.Z);

            return new VertexTransformColor(scale, rotationOffset, rotation, translation, color);
        }

        public virtual void Draw(Camera camera)
        {
            if (geometryChanged || modelChanged)
            {
                throw new RelatusException("The polygon was modified, but ApplyChanges() was never called.", new MethodExpectedException());
            }

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Geometry.Dispose();
                    modelBuffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Polygon()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
