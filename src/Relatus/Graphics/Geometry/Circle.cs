using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class Circle : Polygon
    {
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                Width = radius * 2;
                Height = radius * 2;
            }
        }

        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                Geometry = GeometryManager.CreateHollowCircle(radius, lineWidth);
            }
        }

        private float radius;
        private float lineWidth;

        public Circle(float x, float y, float radius) : base(x, y, 0, radius * 2, radius * 2)
        {
            this.radius = radius;
            AttachGeometry(GeometryManager.GetShapeData(ShapeType.Circle));
            ApplyChanges();
        }
    }
}
