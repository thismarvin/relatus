using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    public static class CollisionHelper
    {
        private struct OverlapInformation
        {
            public Vector2 Normal { get; private set; }
            public float Overlap { get; private set; }
            public bool Valid { get; private set; }

            public OverlapInformation(Vector2 normal, float overlap)
            {
                Normal = normal;
                Overlap = overlap;

                Valid = true;
            }
        }

        public static bool Intersects(PolygonSchema a, PolygonSchema b)
        {
            RectangleF aAABB = CalculateAABB(a.Vertices);
            RectangleF bAABB = CalculateAABB(b.Vertices);

            if (!aAABB.Intersects(bAABB))
                return false;

            OverlapInformation pass0 = CalculateOverlap(a, b);
            OverlapInformation pass1 = CalculateOverlap(b, a);

            if (!pass0.Valid || !pass1.Valid)
                return false;

            return true;
        }

        public static Vector2 GetResolution(PolygonSchema a, PolygonSchema b)
        {
            RectangleF aAABB = CalculateAABB(a.Vertices);
            RectangleF bAABB = CalculateAABB(b.Vertices);

            if (!aAABB.Intersects(bAABB))
                return new Vector2(0, 0);

            OverlapInformation pass0 = CalculateOverlap(a, b);
            OverlapInformation pass1 = CalculateOverlap(b, a);

            if (!pass0.Valid || !pass1.Valid)
                return new Vector2(0, 0);

            OverlapInformation minPass = pass0.Overlap < pass1.Overlap ? pass0 : pass1;
            Vector2 resolution = minPass.Normal * minPass.Overlap;

            Vector2 temp = aAABB.Center - bAABB.Center;
            resolution = Vector2.Dot(temp, resolution) < 0 ? -resolution : resolution;

            return resolution;
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

        private static OverlapInformation CalculateOverlap(PolygonSchema a, PolygonSchema b)
        {
            Vector2 minNormal = Vector2.Zero;
            float minOverlap = float.MaxValue;

            for (int i = 0; i < a.Edges.Length; i++)
            {
                Vector2 normal = new Vector2(
                    -(a.Edges[i].Y2 - a.Edges[i].Y1),
                    a.Edges[i].X2 - a.Edges[i].X1
                );

                normal.Normalize();

                float minProjectionA = float.MaxValue;
                float maxProjectionA = float.MinValue;

                for (int j = 0; j < a.Vertices.Length; j++)
                {
                    float projection = Vector2.Dot(a.Vertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                float minProjectionB = float.MaxValue;
                float maxProjectionB = float.MinValue;

                for (int j = 0; j < b.Vertices.Length; j++)
                {
                    float projection = Vector2.Dot(b.Vertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                float overlap = Math.Min(maxProjectionA, maxProjectionB) - Math.Max(minProjectionA, minProjectionB);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    minNormal = normal;
                }

                if (maxProjectionB < minProjectionA || maxProjectionA < minProjectionB)
                    return new OverlapInformation();
            }

            return new OverlapInformation(minNormal, minOverlap);
        }
    }
}

