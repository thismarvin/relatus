using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using Mimoso.ECS;
using Mimoso.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics
{
    class Polygon
    {
        #region Properties
        public float X
        {
            get => x;
            set
            {
                x = value;
                UpdateTransform();
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                UpdateTransform();
            }
        }
        public float Width
        {
            get => width;
            set
            {
                width = value;
                UpdateTransform();
            }
        }
        public float Height
        {
            get => height;
            set
            {
                height = value;
                UpdateTransform();
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                UpdateTechnique();
            }
        }
        public string Shape
        {
            get => shape;
            set
            {
                shape = value;
                UpdateShape();
            }
        }

        public virtual float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                UpdateTransform();
            }
        }

        public Vector2 RotationOffset
        {
            get => rotationOffset;
            set
            {
                rotationOffset = value;
                UpdateTransform();
            }
        }

        public Vector3 Translation
        {
            get => translation;
            set
            {
                translation = value;
                UpdateTransform();
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                UpdateTransform();
            }
        }

        public ShapeData ShapeData
        {
            get => shapeData;
            set
            {
                if (shapeData != null && !shapeData.Managed)
                {
                    shapeData.Dispose();
                }

                shapeData = value;
                dataChanged = true;
            }
        }
        #endregion

        public Matrix Transform { get; private set; }

        public Vector2 Position { get => new Vector2(X, Y); }
        public Vector2 Center { get => new Vector2(X + Width / 2, Y + Height / 2); }
        public RectangleF Bounds { get => new RectangleF(X, Y, Width, Height); }

        private float x;
        private float y;
        private float width;
        private float height;
        private Color color;
        private string shape;
        private float rotation;
        private Vector2 rotationOffset;
        private Vector3 translation;
        private Vector3 scale;
        private ShapeData shapeData;

        private bool dataChanged;
        protected DynamicVertexBuffer transformBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;
        protected int techniqueIndex;

        private static readonly SpriteBatch spriteBatch;

        static Polygon()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public Polygon(float x, float y, float width, float height, string shape)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.shape = shape;
            color = Color.White;
            rotation = 0;
            rotationOffset = Vector2.Zero;
            translation = Vector3.Zero;
            scale = new Vector3(1);

            UpdateTransform();
            UpdateTechnique();

            if (shape != "Morro_None")
            {
                UpdateShape();
            }
        }

        public Polygon(float x, float y, float width, float height, ShapeType shape) : this(x, y, width, height, $"Morro_{shape.ToString()}")
        {

        }

        public virtual void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
            UpdateTransform();
        }

        public virtual void SetCenter(float x, float y)
        {
            SetPosition(x - Width / 2, y - Height / 2);
        }

        public virtual void SetBounds(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            UpdateTransform();
        }

        public void SetShape(ShapeType shapeType)
        {
            Shape = $"Morro_{shapeType}";
        }

        public VertexTransform GetVertexTransform()
        {
            return new VertexTransform(new CPosition(X, Y), new CDimension(Width, Height), new CTransform(Scale, Rotation, RotationOffset, Translation));
        }

        public VertexTransformColor GetVertexTransformColor()
        {
            return new VertexTransformColor(new CPosition(X, Y), new CDimension(Width, Height), new CTransform(Scale, Rotation, RotationOffset, Translation), new CColor(Color));
        }

        private void UpdateShape()
        {
            dataChanged = true;
            shapeData = GeometryManager.GetShapeData(Shape);
        }

        private void UpdateTransform()
        {
            dataChanged = true;
            Transform =
                Matrix.CreateScale(Width * Scale.X, Height * Scale.Y, 1 * Scale.Z) *

                Matrix.CreateTranslation(-new Vector3(RotationOffset.X, RotationOffset.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *

                Matrix.CreateTranslation(X + Translation.X + RotationOffset.X, Y + Translation.Y + RotationOffset.Y, Translation.Z) *

                Matrix.Identity;
        }

        private void UpdateTechnique()
        {
            dataChanged = true;
            techniqueIndex = Color == Color.White ? 0 : 1;
        }

        private void UpdateBuffer()
        {
            transformBuffer?.Dispose();

            switch (techniqueIndex)
            {
                case 0:
                    transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
                    transformBuffer.SetData(new VertexTransform[] { GetVertexTransform() });
                    break;

                case 1:
                    transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), 1, BufferUsage.WriteOnly);
                    transformBuffer.SetData(new VertexTransformColor[] { GetVertexTransformColor() });
                    break;
            }

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(ShapeData.Geometry),
                new VertexBufferBinding(transformBuffer, 0, 1)
            };
        }

        public virtual void Draw(Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

            spriteBatch.GraphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            spriteBatch.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            spriteBatch.GraphicsDevice.Indices = ShapeData.Indices;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[techniqueIndex].Passes)
            {
                pass.Apply();
                spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, ShapeData.TotalTriangles, 1);
            }
        }
    }
}
