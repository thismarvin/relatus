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

        public static Quad Create(float x, float y, float width, float height, Color color)
        {
            Quad result = new Quad(x, y, width, height);
            result.SetColor(color);

            return result;
        }

        public static Quad Create(float x, float y, float width, float height, float lineWidth, Color color)
        {
            Quad result = new Quad(x, y, width, height);
            result.SetColor(color);
            result.SetLineWidth(lineWidth);

            return result;
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
