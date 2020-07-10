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
            set
            {
                lineWidth = value;
                Geometry = GeometryManager.CreateHollowSquare(Width, Height, lineWidth);
            }
        }

        private float lineWidth;

        public Quad(float x, float y, float width, float height) : base(x, y, width, height)
        {
            AttachGeometry(GeometryManager.GetShapeData(ShapeType.Square));
            ApplyChanges();
        }
    }
}
