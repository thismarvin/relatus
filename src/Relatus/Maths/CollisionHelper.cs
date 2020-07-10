using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{

    public static class CollisionHelper
    {
        private struct OverlapInformation
        {
            public bool Valid { get; private set; }
            public LineSegment Edge { get; private set; }
            public float Overlap { get; private set; }

            public OverlapInformation(LineSegment edge, float overlap)
            {
                Valid = true;
                Edge = edge;
                Overlap = overlap;
            }
        }

        public static Vector2 GetResolution(IShape2D a, IShape2D b)
        {
            RectangleF aAABB = CalculateAABB(a.Vertices);
            RectangleF bAABB = CalculateAABB(b.Vertices);

            if (!aAABB.Intersects(bAABB))
            {
                return new Vector2(0, 0);
            }

            OverlapInformation pass0 = CalculateOverlap(a, b);
            OverlapInformation pass1 = CalculateOverlap(b, a);

            if (!pass0.Valid || !pass1.Valid)
            {
                return new Vector2(0, 0);
            }

            OverlapInformation minPass = pass0.Overlap < pass1.Overlap ? pass0 : pass1;

            Vector2 axis = new Vector2(
                -(minPass.Edge.Y2 - minPass.Edge.Y1),
                minPass.Edge.X2 - minPass.Edge.X1
            );

            float axisLength = axis.Length();
            double angle = Math.Acos(Vector2.Dot(axis, new Vector2(1, 0)) / axisLength);

            double xFactor = Math.Round(axisLength * Math.Cos(angle));
            double yFactor = Math.Round(axisLength * Math.Sin(angle));

            int xResolutionDirection = aAABB.Left > bAABB.Left ? 1 : -1;
            int yResolutionDirection = aAABB.Bottom > bAABB.Bottom ? -1 : 1;

            double xResolution = xFactor == 0 ? 0 : (minPass.Overlap / xFactor) * xResolutionDirection;
            double yResolution = yFactor == 0 ? 0 : (minPass.Overlap / yFactor) * yResolutionDirection;

            return new Vector2((float)xResolution, (float)yResolution);
        }

        private static RectangleF CalculateAABB(Vector2[] vertices)
        {
            float xMin = vertices[0].X;
            float xMax = xMin;
            float yMin = vertices[0].Y;
            float yMax = yMin;

            for (int i = 1; i < vertices.Length; i++)
            {
                xMin = Math.Min(xMin, vertices[i].X);
                xMax = Math.Max(xMax, vertices[i].X);
                yMin = Math.Min(yMin, vertices[i].Y);
                yMax = Math.Max(yMax, vertices[i].Y);
            }

            return new RectangleF(xMin, yMax, xMax - xMin, yMax - yMin);
        }

        private static OverlapInformation CalculateOverlap(IShape2D a, IShape2D b)
        {
            LineSegment edge = new LineSegment(0, 0, 0, 0);
            float minOverlap = float.MaxValue;

            for (int i = 0; i < a.Edges.Length; i++)
            {
                Vector2 normal = new Vector2(
                    -(a.Edges[i].Y2 - a.Edges[i].Y1),
                    a.Edges[i].X2 - a.Edges[i].X1
                );

                float minProjectionA = Vector2.Dot(a.Vertices[0], normal);
                float maxProjectionA = minProjectionA;

                for (int j = 1; j < a.Vertices.Length; j++)
                {
                    float projection = Vector2.Dot(a.Vertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                float minProjectionB = Vector2.Dot(b.Vertices[0], normal);
                float maxProjectionB = minProjectionB;

                for (int j = 1; j < b.Vertices.Length; j++)
                {
                    float projection = Vector2.Dot(b.Vertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                float overlap = Math.Min(maxProjectionA, maxProjectionB) - Math.Max(minProjectionA, minProjectionB);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    edge = a.Edges[i];
                }

                if (maxProjectionB < minProjectionA || maxProjectionA < minProjectionB)
                {
                    return new OverlapInformation(new LineSegment(), -1);
                }
            }

            return new OverlapInformation(edge, minOverlap);
        }
    }
}

