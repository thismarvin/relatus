using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Relatus.Graphics
{
    public static class ImLineSegment
    {
        private const float Epsilon = 0.0001f;

        public static Polygon Create(Vector2[] points, float lineWidth)
        {
            LineInfo info = ProcessPoints(points, lineWidth);

            return new Polygon()
                .SetPosition(info.Boundary.X, info.Boundary.Y, 0)
                .SetDimensions(info.Boundary.Width, info.Boundary.Height)
                .AttachGeometry(CreateShapeData(info, lineWidth));
        }

        public static Polygon Create(float x1, float y1, float x2, float y2, float lineWidth)
        {
            return Create(new Vector2[] { new Vector2(x1, y1), new Vector2(x2, y2) }, lineWidth);
        }

        private class LineInfo
        {
            public Vector2[] RemappedPoints { get; set; }
            public RectangleF Boundary { get; set; }

            public int TotalPoints => RemappedPoints.Length;
            public int TotalVertices => TotalPoints * 2;
            public int TotalTriangles => TotalVertices - 2;
            public int TotalIndices => TotalTriangles * 3;
        }

        private static LineInfo ProcessPoints(Vector2[] points, float lineWidth)
        {
            LineInfo result = new LineInfo();

            float[] xPositions = GetPositions("x");
            float[] yPositions = GetPositions("y");

            float xMin = xPositions.Min();
            float xMax = xPositions.Max();
            float yMin = yPositions.Min();
            float yMax = yPositions.Max();

            xMax = xMax == xMin ? xMax + Epsilon : xMax;
            yMax = yMax == yMin ? yMax + Epsilon : yMax;

            float xRange = xMax - xMin;
            float yRange = yMax - yMin;

            float width = xRange <= 0 ? lineWidth : xRange;
            float height = yRange <= 0 ? lineWidth : yRange;

            result.Boundary = new RectangleF(xMin, yMin, width, height);

            Vector2[] remappedPoints = new Vector2[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                float xRemapped = (float)MathExt.RemapRange(points[i].X, xMin, xMax, 0, 1);
                float yRemapped = (float)MathExt.RemapRange(points[i].Y, yMin, yMax, 0, 1);

                remappedPoints[i] = new Vector2(xRemapped, yRemapped);
            }

            result.RemappedPoints = remappedPoints;

            return result;

            float[] GetPositions(string axis)
            {
                string formattedName = axis.ToLowerInvariant();
                float[] positions = new float[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    if (formattedName == "x")
                    {
                        positions[i] = points[i].X;
                    }
                    else if (formattedName == "y")
                    {
                        positions[i] = points[i].Y;
                    }
                }

                return positions;
            }
        }

        private static Vector3[] CreateVertices(LineInfo info, float lineWidth)
        {
            Vector2[] sloppyVertices = CreateSloppyVertices();
            Vector2[] betterIntersections = FindBetterIntersections(sloppyVertices);

            return CreateFinalVertices(sloppyVertices, betterIntersections);

            Vector2[] CreateSloppyVertices()
            {
                Vector2[] remappedPoints = info.RemappedPoints;
                float width = info.Boundary.Width;
                float height = info.Boundary.Height;
                int totalPoints = info.TotalPoints;

                Vector2[] _sloppyVertices = new Vector2[4 + (totalPoints - 2) * 4];

                float scaledLineWidthX = lineWidth / info.Boundary.Width;
                float scaledLineWidthY = lineWidth / info.Boundary.Height;

                int vertexIndex = 0;

                for (int i = 1; i < totalPoints; i++)
                {
                    float theta = (float)(-MathHelper.PiOver2 + Math.Atan2(remappedPoints[i].Y * height - remappedPoints[i - 1].Y * height, remappedPoints[i].X * width - remappedPoints[i - 1].X * width));

                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i - 1].X - Math.Cos(theta) * (scaledLineWidthX * 0.5f)), (float)(remappedPoints[i - 1].Y - Math.Sin(theta) * (scaledLineWidthY * 0.5f)));
                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i].X - Math.Cos(theta) * (scaledLineWidthX * 0.5f)), (float)(remappedPoints[i].Y - Math.Sin(theta) * (scaledLineWidthY * 0.5f)));
                }

                for (int i = totalPoints - 1; i >= 1; i--)
                {
                    float theta = (float)(-MathHelper.PiOver2 + Math.Atan2(remappedPoints[i].Y * height - remappedPoints[i - 1].Y * height, remappedPoints[i].X * width - remappedPoints[i - 1].X * width));

                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i].X + Math.Cos(theta) * (scaledLineWidthX * 0.5f)), (float)(remappedPoints[i].Y + Math.Sin(theta) * (scaledLineWidthY * 0.5f)));
                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i - 1].X + Math.Cos(theta) * (scaledLineWidthX * 0.5f)), (float)(remappedPoints[i - 1].Y + Math.Sin(theta) * (scaledLineWidthY * 0.5f)));
                }

                return _sloppyVertices;
            }

            Vector2[] FindBetterIntersections(Vector2[] _sloppyVertices)
            {
                int totalPoints = info.TotalPoints;

                Vector2[] _betterIntersections = new Vector2[(totalPoints - 2) * 2];
                int vertexIndex = 0;

                for (int i = 0; i < totalPoints - 2; i++)
                {
                    _betterIntersections[vertexIndex++] = PointOfIntersection(_sloppyVertices[(i * 2)], _sloppyVertices[(i * 2) + 1], _sloppyVertices[(i * 2) + 2], _sloppyVertices[(i * 2) + 3]);
                }

                for (int i = 0; i < totalPoints - 2; i++)
                {
                    _betterIntersections[vertexIndex++] = PointOfIntersection(_sloppyVertices[(totalPoints - 1) * 2 + i * 2], _sloppyVertices[(totalPoints - 1) * 2 + i * 2 + 1], _sloppyVertices[(totalPoints - 1) * 2 + i * 2 + 2], _sloppyVertices[(totalPoints - 1) * 2 + i * 2 + 3]);
                }

                return _betterIntersections;
            }

            Vector3[] CreateFinalVertices(Vector2[] _sloppyVertices, Vector2[] _betterIntersections)
            {
                int totalPoints = info.TotalPoints;

                Vector3[] vertices = new Vector3[info.TotalVertices];
                int vertexIndex = 0;
                int intersectionIndex = 0;

                for (int i = 0; i < totalPoints; i++)
                {
                    if (i == 0)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[0].X, _sloppyVertices[0].Y, 0);
                    }
                    else if (i == totalPoints - 1)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[2 * totalPoints - 3].X, _sloppyVertices[2 * totalPoints - 3].Y, 0);
                    }
                    else
                    {
                        vertices[vertexIndex++] = new Vector3(_betterIntersections[intersectionIndex].X, _betterIntersections[intersectionIndex].Y, 0);
                        intersectionIndex++;
                    }
                }

                for (int i = 0; i < totalPoints; i++)
                {
                    if (i == 0)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[2 * totalPoints - 3 + 1].X, _sloppyVertices[2 * totalPoints - 3 + 1].Y, 0);
                    }
                    else if (i == totalPoints - 1)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[^1].X, _sloppyVertices[^1].Y, 0);
                    }
                    else
                    {
                        vertices[vertexIndex++] = new Vector3(_betterIntersections[intersectionIndex].X, _betterIntersections[intersectionIndex].Y, 0);
                        intersectionIndex++;
                    }
                }

                return vertices;
            }

            static Vector2 PointOfIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
            {
                // Prevents a slope of NaN.
                if (a2.X == a1.X)
                {
                    a2.X += Epsilon;
                }
                if (b2.X == b1.X)
                {
                    b2.X += Epsilon;
                }

                float aSlope = (a2.Y - a1.Y) / (a2.X - a1.X);
                float aYIntercept = a1.Y - aSlope * a1.X;

                float bSlope = (b2.Y - b1.Y) / (b2.X - b1.X);
                float bYIntercept = b1.Y - bSlope * b1.X;

                // Handles parallel lines.
                if (Math.Round(aSlope - bSlope, 3) == 0)
                    return a2;

                float x = (bYIntercept - aYIntercept) / (aSlope - bSlope);
                float y = aSlope * x + aYIntercept;

                return new Vector2(x, y);
            }
        }

        private static short[] CreateIndices(LineInfo info)
        {
            int totalPoints = info.RemappedPoints.Length;

            short[] indices = new short[info.TotalIndices];

            int i = 0;
            int j = 0;
            for (; i < totalPoints - 1; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(totalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i + 1);
                j += 3;
            }

            i = 0;
            for (; i < totalPoints - 1; i++)
            {
                indices[j] = (short)(totalPoints * 2 - 1 - i);
                indices[j + 1] = (short)(totalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i);
                j += 3;
            }

            return indices;
        }

        private static GeometryData CreateShapeData(LineInfo info, float lineWidth)
        {
            return new GeometryData(new Mesh(CreateVertices(info, lineWidth), CreateIndices(info)));
        }
    }
}
