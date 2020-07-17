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

        public Circle(float x, float y, float radius)
        {
            this.radius = radius;

            SetBounds(x, y, this.radius * 2, this.radius * 2);
            AttachGeometry(GeometryManager.GetShapeData(ShapeType.Circle));
            ApplyChanges();
        }
    }
}
