using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class Polyhedron : Geometry
    {
        public float Width
        {
            get => transform.Scale.X;
            set => SetDimensions(value, transform.Scale.Y, transform.Scale.Z);
        }
        public float Height
        {
            get => transform.Scale.Y;
            set => SetDimensions(transform.Scale.X, value, transform.Scale.Z);
        }
        public float Depth
        {
            get => transform.Scale.Z;
            set => SetDimensions(transform.Scale.X, transform.Scale.Y, value);
        }

        //public BoundingBox AABB => new BoundingBox(transform.Translation, transform.Translation + transform.Scale);

        public Polyhedron() : base()
        {

        }

        public virtual Renderable SetDimensions(float width, float height, float depth)
        {
            transform.Scale = new Vector3(width, height, depth);

            return this;
        }
    }
}
