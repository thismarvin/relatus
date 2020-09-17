using Microsoft.Xna.Framework;

namespace Relatus
{
    public class OrthographicCamera : Camera
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public RectangleF Bounds => new RectangleF(Position.X, Position.Y, Width, Height);

        public OrthographicCamera(float width, float height, float near, float far) : base()
        {
            Width = width;
            Height = height;
            this.near = near;
            this.far = far;

            projection = Matrix.CreateOrthographic(Width, Height, near, far);
        }
    }
}
