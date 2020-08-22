using Microsoft.Xna.Framework;
using System;

namespace Relatus.Graphics
{
    public class Polygon : IDisposable
    {
        #region Properties
        public float Width
        {
            get => width;
            set => SetDimensions(value, height);
        }
        public float Height
        {
            get => height;
            set => SetDimensions(width, value);
        }
        public Vector3 Position
        {
            get => position;
            set => SetPosition(value);
        }
        public Color Color
        {
            get => color;
            set => SetColor(value);
        }
        public virtual Vector3 Rotation
        {
            get => rotation;
            set => SetRotation(value.X, value.Y, value.Z);
        }
        public Vector3 Origin
        {
            get => origin;
            set => SetOrigin(value);
        }
        public Vector3 Translation
        {
            get => translation;
            set => SetTranslation(value);
        }
        public Vector3 Scale
        {
            get => scale;
            set => SetScale(value);
        }
        public GeometryData Geometry
        {
            get => geometry;
            set => AttachGeometry(value);
        }
        public RenderOptions RenderOptions
        {
            get => renderOptions;
            set => SetRenderOptions(value);
        }
        #endregion

        public Vector3 Center
        {
            get => new Vector3(position.X + width * 0.5f, position.Y - height * 0.5f, position.Z);
            set => SetCenter(value.X, value.Y);
        }
        public RectangleF Bounds
        {
            get => new RectangleF(position.X, position.Y, width, height);
        }
        public float X
        {
            get => position.X;
            set => SetPosition(value, position.Y, position.Z);
        }
        public float Y
        {
            get => position.Y;
            set => SetPosition(position.X, value, position.Z);
        }
        public float Z
        {
            get => position.Z;
            set => SetPosition(position.X, position.Y, value);
        }

        private float width;
        private float height;
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 origin;
        private Vector3 translation;
        private Vector3 scale;
        private Color color;
        private GeometryData geometry;
        private RenderOptions renderOptions;

        private bool transformNeedsUpdating;
        private Matrix transformCache;

        public Polygon()
        {
            color = Color.White;
            scale = Vector3.One;
            renderOptions = new RenderOptions();

            transformNeedsUpdating = true;
            transformCache = Matrix.Identity;
        }

        public static Polygon Create()
        {
            return new Polygon();
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

            return this;
        }

        public virtual Polygon SetPosition(Vector3 position)
        {
            this.position = position;

            transformNeedsUpdating = true;

            return this;
        }

        public Polygon SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public virtual Polygon SetDimensions(float width, float height)
        {
            this.width = width;
            this.height = height;

            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetColor(Color color)
        {
            this.color = color;

            return this;
        }

        public Polygon SetColor(int r, int g, int b, float a = 1)
        {
            this.color = new Color(r, g, b) * a;

            return this;
        }

        public virtual Polygon SetTranslation(Vector3 translation)
        {
            this.translation = translation;

            transformNeedsUpdating = true;

            return this;
        }

        public Polygon SetTranslation(float x, float y, float z)
        {
            return SetTranslation(new Vector3(x, y, z));
        }

        public virtual Polygon SetScale(Vector3 scale)
        {
            this.scale = scale;

            transformNeedsUpdating = true;

            return this;
        }

        public Polygon SetScale(float x, float y, float z)
        {
            return SetScale(new Vector3(x, y, z));
        }

        public virtual Polygon SetOrigin(Vector3 origin)
        {
            this.origin = origin;

            transformNeedsUpdating = true;

            return this;
        }

        public Polygon SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public virtual Polygon SetRotation(float roll, float pitch, float yaw)
        {
            rotation = new Vector3(roll, pitch, yaw);

            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetRenderOptions(RenderOptions options)
        {
            renderOptions = options;

            return this;
        }

        public virtual Polygon SetCenter(float x, float y)
        {
            position = new Vector3(x - width * 0.5f, y + height * 0.5f, position.Z);

            transformNeedsUpdating = true;

            return this;
        }

        public Matrix CalculateTransform()
        {
            if (transformNeedsUpdating)
            {
                transformNeedsUpdating = false;

                transformCache =
                Matrix.CreateScale(width * scale.X, height * scale.Y, scale.Z) *
                Matrix.CreateTranslation(-origin) *
                Matrix.CreateRotationZ(rotation.Z) *
                Matrix.CreateRotationY(rotation.Y) *
                Matrix.CreateRotationX(rotation.X) *
                Matrix.CreateTranslation(origin + translation + Position);
            }

            return transformCache;
        }

        public PolygonSchema CalculatePolygonSchema()
        {
            int totalVertices = Geometry.TotalVertices;
            Matrix polygonTransform = CalculateTransform();

            Vector2[] transformedVertices = new Vector2[totalVertices];

            for (int i = 0; i < totalVertices; i++)
            {
                transformedVertices[i] = Vector2.Transform(new Vector2(Geometry.Mesh.Vertices[i].X, Geometry.Mesh.Vertices[i].Y), polygonTransform);
            }

            LineSegment[] transformedLineSegments = new LineSegment[totalVertices];

            for (int i = 0; i < totalVertices - 1; i++)
            {
                transformedLineSegments[i] = new LineSegment(transformedVertices[i + 1].X, transformedVertices[i + 1].Y, transformedVertices[i].X, transformedVertices[i].Y);
            }

            transformedLineSegments[totalVertices - 1] = new LineSegment(transformedVertices[0].X, transformedVertices[0].Y, transformedVertices[totalVertices - 1].X, transformedVertices[totalVertices - 1].Y);

            return new PolygonSchema(transformedVertices, transformedLineSegments);
        }

        internal VertexTransform GetVertexTransform()
        {
            Vector3 scale = new Vector3(width * this.scale.X, height * this.scale.Y, this.scale.Z);

            return new VertexTransform(position + translation, scale, origin, rotation);
        }

        internal VertexColor GetVertexColor()
        {
            return new VertexColor(color);
        }

        protected virtual void OnDispose()
        {

        }

        #region IDisposable Support
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!Geometry.Managed)
                    {
                        Geometry.Dispose();
                    }

                    OnDispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
