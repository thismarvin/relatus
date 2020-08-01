using System;
using System.Collections.Generic;
using System.Text;

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

        public Quad(float x, float y, float width, float height)
        {
            SetBounds(x, y, width, height);
            AttachGeometry(GeometryManager.GetShapeData(ShapeType.Square));
            ApplyChanges();
        }

        public Quad SetLineWidth(float lineWidth)
        {
            if (this.lineWidth == lineWidth)
                return this;

            this.lineWidth = lineWidth;
            AttachGeometry(GeometryManager.CreateHollowSquare(Width, Height, lineWidth));

            return this;
        }
    }
}
