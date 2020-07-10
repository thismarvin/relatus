using Microsoft.Xna.Framework;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    internal static class PolygonHelper
    {
        #region Collision Handling
        internal static SchemaShape2D GetCollisionInformation(this Polygon polygon)
        {
            Vector2[] transformedVertices = CalculateTransformedVertices(polygon);
            LineSegment[] transformedLineSegments = CalculateTransformedLineSegments(polygon, transformedVertices);

            return new SchemaShape2D(transformedVertices, transformedLineSegments);
        }

        private static Vector2[] CalculateTransformedVertices(Polygon polygon)
        {
            Vector2[] result = new Vector2[polygon.Geometry.TotalVertices];
            Matrix polygonTransform = polygon.CalculateTransform();

            for (int i = 0; i < polygon.Geometry.TotalVertices; i++)
            {
                result[i] = Vector2.Transform(new Vector2(polygon.Geometry.Vertices[i].X, polygon.Geometry.Vertices[i].Y), polygonTransform);
            }

            return result;
        }

        private static LineSegment[] CalculateTransformedLineSegments(Polygon polygon, Vector2[] transformedVertices)
        {
            int totalVertices = polygon.Geometry.TotalVertices;
            LineSegment[] result = new LineSegment[totalVertices];

            result[0] = new LineSegment(transformedVertices[totalVertices - 1].X, transformedVertices[totalVertices - 1].Y, transformedVertices[0].X, transformedVertices[0].Y);

            for (int i = 1; i < totalVertices; i++)
            {
                result[i] = new LineSegment(transformedVertices[i - 1].X, transformedVertices[i - 1].Y, transformedVertices[i].X, transformedVertices[i].Y);
            }

            return result;
        }
        #endregion
    }
}
