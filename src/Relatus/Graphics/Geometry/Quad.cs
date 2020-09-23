using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class Quad : Polygon
    {
        public float LineWidth
        {
            get => lineWidth;
            set => SetLineWidth(value);
        }

        private float lineWidth;

        private readonly static GeometryData geometry;

        static Quad()
        {
            geometry = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public Quad()
        {
            AttachGeometry(geometry);
        }

        public Quad(float x, float y, float width, float height)
        {
            SetPosition(x, y, 0);
            SetDimensions(width, height);
            AttachGeometry(geometry);
        }

        public static Quad Create(float x, float y, float width, float height, Color tint)
        {
            Quad result = new Quad(x, y, width, height);
            result.SetTint(tint);

            return result;
        }

        public static Quad Create(float x, float y, float width, float height, float lineWidth, Color tint)
        {
            Quad result = new Quad(x, y, width, height);
            result.SetTint(tint);
            result.SetLineWidth(lineWidth);

            return result;
        }

        public override Renderable SetDimensions(float width, float height)
        {
            if (width == Width && height == Height)
                return this;

            base.SetDimensions(width, height);

            if (lineWidth != 0)
            {
                AttachGeometry(GeometryManager.CreateHollowSquare(Width, Height, lineWidth));
            }

            return this;
        }

        public Polygon SetLineWidth(float lineWidth)
        {
            if (this.lineWidth == lineWidth)
                return this;

            this.lineWidth = lineWidth;
            AttachGeometry(GeometryManager.CreateHollowSquare(Width, Height, this.lineWidth));

            return this;
        }
    }
}
