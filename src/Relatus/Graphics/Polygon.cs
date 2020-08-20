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
            set => SetOrigin(value.X, value.Y, value.Z);
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

        public Vector3 Position
        {
            get => new Vector3(x, y, z);
            set => SetPosition(value.X, value.Y, value.Z);
        }

        // I just want to note that although this is a Vector3, it is really treated like a Vector2.
        public Vector3 Center
        {
            get => new Vector3(x + width * 0.5f, y - height * 0.5f, z);
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
        private Vector3 rotation;
        private Vector3 origin;
        private Vector3 translation;
        private Vector3 scale;
        private GeometryData geometry;
        private RenderOptions renderOptions;

        private bool geometryChanged;
        private bool modelChanged;
        private bool colorChanged;
        private bool transformNeedsUpdating;
        private Matrix transformCache;
        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Effect polygonShader;
        private static readonly EffectPass polygonPass;

        static Polygon()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            polygonShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            polygonPass = polygonShader.Techniques[1].Passes[0];
        }

        public Polygon()
        {
            color = Color.White;
            scale = Vector3.One;
            renderOptions = new RenderOptions();

            geometryChanged = true;
            modelChanged = true;
            colorChanged = true;

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
            geometryChanged = true;

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

        public virtual Polygon SetDimensions(float width, float height)
        {
            this.width = width;
            this.height = height;

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetColor(Color color)
        {
            this.color = color;

            colorChanged = true;

            return this;
        }

        public virtual Polygon SetTranslation(float x, float y, float z)
        {
            translation = new Vector3(x, y, z);

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetScale(float x, float y, float z)
        {
            scale = new Vector3(x, y, z);

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetOrigin(float x, float y, float z)
        {
            origin = new Vector3(x, y, z);

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetRotation(float roll, float pitch, float yaw)
        {
            rotation = new Vector3(roll, pitch, yaw);

            modelChanged = true;
            transformNeedsUpdating = true;

            return this;
        }

        public virtual Polygon SetRenderOptions(RenderOptions options)
        {
            renderOptions = options;

            return this;
        }

        public Polygon ApplyChanges()
        {
            if (!geometryChanged && !modelChanged && !colorChanged)
                return this;

            if (modelChanged)
            {
                transformBuffer?.Dispose();
                transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
                transformBuffer.SetData(new VertexTransform[] { GetVertexTransform() });
            }

            if (colorChanged)
            {
                colorBuffer?.Dispose();
                colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), 1, BufferUsage.WriteOnly);
                colorBuffer.SetData(new VertexColor[] { GetVertexColor() });
            }

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(geometry.VertexBuffer),
                new VertexBufferBinding(transformBuffer, 0, 1),
                new VertexBufferBinding(colorBuffer, 0, 1)
            };

            geometryChanged = false;
            modelChanged = false;
            colorChanged = false;

            return this;
        }

        public virtual Polygon SetCenter(float x, float y)
        {
            this.x = x - width * 0.5f;
            this.y = y + height * 0.5f;

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
            Vector3 translation = new Vector3(x + this.translation.X, y + this.translation.Y, z + this.translation.Z);
            Vector3 scale = new Vector3(width * this.scale.X, height * this.scale.Y, this.scale.Z);

            return new VertexTransform(translation, scale, origin, rotation);
        }

        internal VertexColor GetVertexColor()
        {
            return new VertexColor(color);
        }

        public virtual void Draw(Camera camera)
        {
            if (geometryChanged || modelChanged || colorChanged)
                throw new RelatusException("The polygon was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = renderOptions.SamplerState;
            graphicsDevice.BlendState = renderOptions.BlendState;
            graphicsDevice.DepthStencilState = renderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;

            polygonShader.Parameters["WVP"].SetValue(camera.WVP);

            polygonPass.Apply();

            if (renderOptions.Effect == null)
            {
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
            }
            else
            {
                foreach (EffectPass pass in renderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
                }
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
                    transformBuffer.Dispose();
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
