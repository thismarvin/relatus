using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{
    public struct LineSegment
    {
        public float X1 { get; private set; }
        public float Y1 { get; private set; }
        public float X2 { get; private set; }
        public float Y2 { get; private set; }

        public LineSegment(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public IntersectionInformation GetIntersectionInformation(LineSegment segment)
        {
            float x3 = segment.X1;
            float y3 = segment.Y1;
            float x4 = segment.X2;
            float y4 = segment.Y2;

            float denominator = (X1 - X2) * (y3 - y4) - (Y1 - Y2) * (x3 - x4);

            if (denominator == 0)
            {
                return new IntersectionInformation();
            }

            float numerator = (X1 - x3) * (y3 - y4) - (Y1 - y3) * (x3 - x4);
            float t = numerator / denominator;

            numerator = (X1 - X2) * (Y1 - y3) - (Y1 - Y2) * (X1 - x3);
            float u = -numerator / denominator;

            return new IntersectionInformation(t, u, IntersectionPoint(t));
        }

        public bool Intersects(LineSegment segment)
        {
            IntersectionInformation result = GetIntersectionInformation(segment);

            return result.Valid ? result.Intersected : false;
        }

        private Vector2 IntersectionPoint(float t)
        {
            return new Vector2(X1 + t * (X2 - X1), Y1 + t * (Y2 - Y1));
        }
    }
}
