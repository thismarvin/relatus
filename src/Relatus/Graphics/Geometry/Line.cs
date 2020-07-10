using Microsoft.Xna.Framework;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Relatus.Graphics
{
    public class Line
    {
        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                UpdateShapeData();
            }
        }

        public Color Color
        {
            get => line.Color;
            set => line.Color = value;
        }

        public Vector2 StartPoint { get { return points[0]; } }
        public Vector2 EndPoint { get { return points[points.Length - 1]; } }

        private float x;
        private float y;
        private float width;
        private float height;
        private float lineWidth;

        private Vector2[] points;
        private Vector2[] remappedPoints;
        private readonly Polygon line;

        private int TotalPoints { get => points.Length; }
        private int TotalVertices { get => TotalPoints * 2; }
        private int TotalTriangles { get => TotalVertices - 2; }
        private int TotalIndices { get => TotalTriangles * 3; }

        public Line(float x1, float y1, float x2, float y2)
        {
            points = new Vector2[]
            {
                new Vector2(x1, y1),
                new Vector2(x2, y2),
            };

            lineWidth = 1;

            ProcessPoints();

            line = new Polygon(x, y, width, height)
            {
                Geometry = CreateShapeData()
            };
        }

        public Line(Vector2[] points)
        {
            lineWidth = 1;
            line = new Polygon(x, y, width, height);

            SetPoints(points);
        }

        public void SetPoints(Vector2[] points)
        {
            if (points.Length < 2)
                throw new RelatusException("A line requires at least two points.", new ArgumentException());

            this.points = points;
            UpdateShapeData();
        }

        public void SetStartPoint(float x, float y)
        {
            points[0] = new Vector2(x, y);
            UpdateShapeData();
        }

        public void SetEndPoint(float x, float y)
        {
            points[points.Length - 1] = new Vector2(x, y);
            UpdateShapeData();
        }

        private void ProcessPoints()
        {
            remappedPoints = new Vector2[TotalPoints];

            float[] xPositions = GetPositions("x");
            float[] yPositions = GetPositions("y");

            float xMin = xPositions.Min();
            float xMax = xPositions.Max();
            float yMin = yPositions.Min();
            float yMax = yPositions.Max();

            xMax = xMax == xMin ? xMax + 0.0001f : xMax;
            yMax = yMax == yMin ? yMax + 0.0001f : yMax;

            x = xMin;
            y = yMin;

            float xRange = xMax - xMin;
            float yRange = yMax - yMin;

            width = xRange <= 0 ? LineWidth : xRange;
            height = yRange <= 0 ? LineWidth : yRange;

            float xRemapped;
            float yRemapped;
            for (int i = 0; i < TotalPoints; i++)
            {
                xRemapped = (float)MoreMaths.RemapRange(points[i].X, xMin, xMax, 0, 1);
                yRemapped = (float)MoreMaths.RemapRange(points[i].Y, yMin, yMax, 0, 1);

                remappedPoints[i] = new Vector2(xRemapped, yRemapped);
            }

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

        private Vector3[] CreateVertices()
        {
            Vector2[] sloppyVertices = CreateSloppyVertices();
            Vector2[] betterIntersections = FindBetterIntersections(sloppyVertices);

            return CreateFinalVertices(sloppyVertices, betterIntersections);

            Vector2[] CreateSloppyVertices()
            {
                Vector2[] _sloppyVertices = new Vector2[4 + (TotalPoints - 2) * 4];
                int vertexIndex = 0;

                float theta;
                float scaledLineWidthX = LineWidth / width;
                float scaledLineWidthY = LineWidth / height;

                for (int i = 1; i < TotalPoints; i++)
                {
                    theta = (float)(-MathHelper.PiOver2 + Math.Atan2(remappedPoints[i].Y * height - remappedPoints[i - 1].Y * height, remappedPoints[i].X * width - remappedPoints[i - 1].X * width));

                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i - 1].X - Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(remappedPoints[i - 1].Y - Math.Sin(theta) * (scaledLineWidthY / 2)));
                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i].X - Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(remappedPoints[i].Y - Math.Sin(theta) * (scaledLineWidthY / 2)));
                }

                for (int i = TotalPoints - 1; i >= 1; i--)
                {
                    theta = (float)(-MathHelper.PiOver2 + Math.Atan2(remappedPoints[i].Y * height - remappedPoints[i - 1].Y * height, remappedPoints[i].X * width - remappedPoints[i - 1].X * width));

                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i].X + Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(remappedPoints[i].Y + Math.Sin(theta) * (scaledLineWidthY / 2)));
                    _sloppyVertices[vertexIndex++] = new Vector2((float)(remappedPoints[i - 1].X + Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(remappedPoints[i - 1].Y + Math.Sin(theta) * (scaledLineWidthY / 2)));
                }

                return _sloppyVertices;
            }

            Vector2[] FindBetterIntersections(Vector2[] _sloppyVertices)
            {
                Vector2[] _betterIntersections = new Vector2[(TotalPoints - 2) * 2];
                int vertexIndex = 0;

                for (int i = 0; i < TotalPoints - 2; i++)
                {
                    _betterIntersections[vertexIndex++] = PointOfIntersection(_sloppyVertices[(i * 2)], _sloppyVertices[(i * 2) + 1], _sloppyVertices[(i * 2) + 2], _sloppyVertices[(i * 2) + 3]);
                }

                for (int i = 0; i < TotalPoints - 2; i++)
                {
                    _betterIntersections[vertexIndex++] = PointOfIntersection(_sloppyVertices[(TotalPoints - 1) * 2 + i * 2], _sloppyVertices[(TotalPoints - 1) * 2 + i * 2 + 1], _sloppyVertices[(TotalPoints - 1) * 2 + i * 2 + 2], _sloppyVertices[(TotalPoints - 1) * 2 + i * 2 + 3]);
                }

                return _betterIntersections;
            }

            Vector3[] CreateFinalVertices(Vector2[] _sloppyVertices, Vector2[] _betterIntersections)
            {
                Vector3[] vertices = new Vector3[TotalVertices];
                int vertexIndex = 0;
                int intersectionIndex = 0;

                for (int i = 0; i < TotalPoints; i++)
                {
                    if (i == 0)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[0].X, _sloppyVertices[0].Y, 0);
                    }
                    else if (i == TotalPoints - 1)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[2 * TotalPoints - 3].X, _sloppyVertices[2 * TotalPoints - 3].Y, 0);
                    }
                    else
                    {
                        vertices[vertexIndex++] = new Vector3(_betterIntersections[intersectionIndex].X, _betterIntersections[intersectionIndex].Y, 0);
                        intersectionIndex++;
                    }
                }

                for (int i = 0; i < TotalPoints; i++)
                {
                    if (i == 0)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[2 * TotalPoints - 3 + 1].X, _sloppyVertices[2 * TotalPoints - 3 + 1].Y, 0);
                    }
                    else if (i == TotalPoints - 1)
                    {
                        vertices[vertexIndex++] = new Vector3(_sloppyVertices[_sloppyVertices.Length - 1].X, _sloppyVertices[_sloppyVertices.Length - 1].Y, 0);
                    }
                    else
                    {
                        vertices[vertexIndex++] = new Vector3(_betterIntersections[intersectionIndex].X, _betterIntersections[intersectionIndex].Y, 0);
                        intersectionIndex++;
                    }
                }

                return vertices;
            }

            Vector2 PointOfIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
            {
                // Prevents a slope of NaN.
                if (a2.X == a1.X)
                {
                    a2.X += 0.0001f;
                }
                if (b2.X == b1.X)
                {
                    b2.X += 0.0001f;
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

        private short[] CreateIndices()
        {
            short[] indices = new short[TotalIndices];

            int i = 0;
            int j = 0;
            for (; i < TotalPoints - 1; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(TotalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i + 1);
                j += 3;
            }

            i = 0;
            for (; i < TotalPoints - 1; i++)
            {
                indices[j] = (short)(TotalPoints * 2 - 1 - i);
                indices[j + 1] = (short)(TotalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i);
                j += 3;
            }

            return indices;
        }

        private GeometryData CreateShapeData()
        {
            return new GeometryData(CreateVertices(), CreateIndices());
        }

        private void UpdateShapeData()
        {
            ProcessPoints();
            line.Geometry = CreateShapeData();
            line.SetBounds(x, y, width, height);
        }

        public void Draw(Camera camera)
        {
            line.Draw(camera);
        }
    }
}
