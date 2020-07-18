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
                if (radius == value)
                    return;

                radius = value;                
                SetBounds(X, Y, radius * 2, radius * 2);
            }
        }

        public float LineWidth
        {
            get => lineWidth;
            set
            {
                if (lineWidth == value)
                    return;

                lineWidth = value;
                AttachGeometry(GeometryManager.CreateHollowCircle(radius, lineWidth));
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
