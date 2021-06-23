using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class Circle : Polygon
    {
        public float Radius
        {
            get => radius;
            set => SetRadius(value);
        }

        public float LineWidth
        {
            get => lineWidth;
            set => SetLineWidth(value);
        }

        private float radius;
        private float lineWidth;

        private static readonly GeometryData geometry;

        static Circle()
        {
            geometry = GeometryManager.GetShapeData(ShapeType.Circle);
        }

        public Circle()
        {
            AttachGeometry(geometry);
        }

        public Circle(float x, float y, float radius)
        {
            this.radius = radius;

            SetPosition(x, y, 0);
            SetDimensions(this.radius * 2, this.radius * 2);
            AttachGeometry(geometry);
        }

        public static Circle Create(float x, float y, float radius, Color tint)
        {
            Circle result = new Circle(x, y, radius);
            result.SetTint(tint);

            return result;
        }

        public static Circle Create(float x, float y, float radius, float lineWidth, Color tint)
        {
            Circle result = new Circle(x, y, radius);
            result.SetTint(tint);
            result.SetLineWidth(lineWidth);

            return result;
        }

        public Polygon SetRadius(float radius)
        {
            if (this.radius == radius)
                return this;

            this.radius = radius;
            SetDimensions(radius * 2, radius * 2);

            if (lineWidth != 0)
            {
                AttachGeometry(GeometryManager.CreateHollowCircle(this.radius, lineWidth));
            }

            return this;
        }

        public Polygon SetLineWidth(float lineWidth)
        {
            if (this.lineWidth == lineWidth)
                return this;

            this.lineWidth = lineWidth;
            AttachGeometry(GeometryManager.CreateHollowCircle(radius, this.lineWidth));

            return this;
        }
    }
}
