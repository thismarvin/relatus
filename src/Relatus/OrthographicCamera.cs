using System;
using Microsoft.Xna.Framework;

namespace Relatus
{
    public class OrthographicCamera : Camera
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectangleF Bounds => new RectangleF(Position.X, Position.Y, Width, Height);

        public OrthographicCamera() : base()
        {
        }

        public OrthographicCamera SetProjection(float width, float height, float near, float far)
        {
            Width = width;
            Height = height;
            this.near = near;
            this.far = far;

            Projection = Matrix.CreateOrthographic(Width, Height, this.near, this.far);

            return this;
        }

        public OrthographicCamera SetProjection(float left, float right, float bottom, float top, float near, float far)
        {
            Width = Math.Abs(right - left);
            Height = Math.Abs(top - bottom);
            this.near = near;
            this.far = far;

            Projection = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, this.near, this.far);

            return this;
        }
    }
}
