using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Basic shape types that have been created and registered by default.
    /// </summary>
    public enum ShapeType
    {
        Triangle,
        RightTriangle,
        Square,
        Star,
        Circle
    }

    /// <summary>
    /// Handles the entire life cycle of any registered <see cref="GeometryData"/>, and provides functionality to create
    /// additional basic shape data.
    /// </summary>
    public static class GeometryManager
    {
        public static Effect PolygonShader { get; private set; }

        private static readonly ResourceHandler<GeometryData> shapes;

        static GeometryManager()
        {
            PolygonShader = AssetManager.GetEffect("PolygonShader").Clone();
            shapes = new ResourceHandler<GeometryData>();

            RegisterTriangle();
            RegisterRightTriangle();
            RegisterSquare();
            RegisterCircle();
            RegisterStar();
        }

        #region Handle Shape Data
        /// <summary>
        /// Register <see cref="GeometryData"/> to be managed by Relatus.
        /// </summary>
        /// <param name="name">The name that the shape data being registered will be referenced as.</param>
        /// <param name="shapeData">The shape data you want to be registered.</param>
        public static void RegisterShapeData(string name, GeometryData shapeData)
        {
            shapes.Register(name, shapeData);
        }

        /// <summary>
        /// Get <see cref="GeometryData"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to shape data that was already registered.</param>
        /// <returns>The registered shape data with the given name.</returns>
        public static GeometryData GetShapeData(string name)
        {
            return shapes.Get(name);
        }

        /// <summary>
        /// Get <see cref="GeometryData"/> that was registered by Relatus.
        /// </summary>
        /// <param name="shapeType">The basic shape data you want to get.</param>
        /// <returns>The registered shape data with the given name.</returns>
        public static GeometryData GetShapeData(ShapeType shapeType)
        {
            return GetShapeData($"Relatus_{shapeType}");
        }

        /// <summary>
        /// Remove registered <see cref="GeometryData"/>.
        /// </summary>
        /// <param name="name">The name given to shape data that was already registered.</param>
        public static void RemoveShapeData(string name)
        {
            shapes.Remove(name);
        }
        #endregion

        /// <summary>
        /// Sets <see cref="PolygonShader"/>'s "WorldViewProjection" parameter to the camera's <see cref="Camera.WorldViewProjection"/>.
        /// </summary>
        /// <param name="camera">The target camera.</param>
        public static void SetupPolygonShader(Camera camera)
        {
            PolygonShader.Parameters["WorldViewProjection"].SetValue(camera.WorldViewProjection);
        }

        /// <summary>
        /// Creates a filled regular polygon given a specific amount of vertices.
        /// </summary>
        /// <param name="totalVertices">The total amount of vertices the regular polygon will have.</param>
        /// <returns>Generated shape data of your regular polygon.</returns>
        public static GeometryData CreateRegularPolygon(int totalVertices)
        {
            if (totalVertices <= 2)
                throw new RelatusException("A polygon must have at least 3 vertices.", new ArgumentException());

            List<Vector3> vertices = new List<Vector3>();
            List<short> indices = new List<short>();
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;
            float angleIncrement = MathHelper.TwoPi / totalVertices;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0));

                if (vertices.Count >= totalVertices)
                    break;
            }

            int j = 1;
            for (int i = 0; i < totalIndices; i += 3)
            {
                indices.Add(0);
                indices.Add((short)(j + 1));
                indices.Add((short)j);
                j++;
            }

            return new GeometryData(vertices.ToArray(), indices.ToArray());
        }

        /// <summary>
        /// Creates a hollow regular polygon given a specific amount of vertices, width, height, and line width.
        /// </summary>
        /// <param name="totalVertices">The total amount of vertices the regular polygon will have.</param>
        /// <param name="width">The desired width of the polygon. (In order for the line width to be accurate, it must be scaled by the desired width).</param>
        /// <param name="height">The desired height of the polygon. (In order for the line width to be accurate, it must be scaled by the desired height).</param>
        /// <param name="lineWidth">The desired line width of the polygon.</param>
        /// <returns>Generated shape data of your hollow regular polygon.</returns>
        public static GeometryData CreateHollowRegularPolygon(int totalVertices, float width, float height, float lineWidth)
        {
            if (totalVertices <= 2)
                throw new RelatusException("A polygon must have at least 3 vertices.", new ArgumentException());

            int initialTotalVertices = totalVertices;
            int _totalVertices = initialTotalVertices * 2;
            int totalTriangles = initialTotalVertices * 2;
            int totalIndices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[_totalVertices];

            float angleIncrement = MathHelper.TwoPi / totalVertices;

            float theta;
            if (initialTotalVertices == 3)
            {
                theta = (float)Math.PI / 6;
            }
            else
            {
                theta = MathHelper.TwoPi / initialTotalVertices;
                theta = MathHelper.Pi - theta;
                theta /= 2;
            }

            float scaledLineWidthX = (lineWidth / width) / (float)Math.Sin(theta);
            float scaledLineWidthY = (lineWidth / height) / (float)Math.Sin(theta);
            int vertexIndex = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new Vector3(0.5f + (float)Math.Cos(i) * (0.5f - scaledLineWidthX), 0.5f + (float)Math.Sin(i) * (0.5f - scaledLineWidthY), 0);

                if (vertexIndex >= vertices.Length / 2)
                    break;
            }

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0);

                if (vertexIndex >= vertices.Length)
                    break;
            }

            short[] indices = CreateHollowIndices(_totalVertices, totalIndices);

            return new GeometryData(vertices, indices);
        }

        /// <summary>
        /// Creates a hollow square given a specific width, height, and line width.
        /// </summary>
        /// <param name="width">The desired width of the polygon. (In order for the line width to be accurate, it must be scaled by the desired width).</param>
        /// <param name="height">The desired height of the polygon. (In order for the line width to be accurate, it must be scaled by the desired height).</param>
        /// <param name="lineWidth">The desired line width of the polygon.</param>
        /// <returns>Generated shape data of your hollow square.</returns>
        public static GeometryData CreateHollowSquare(float width, float height, float lineWidth)
        {
            int initialTotalVertices = 4;
            int totalVertices = initialTotalVertices * 2;
            int totalTriangles = initialTotalVertices * 2;
            int totalIndices = totalTriangles * 3;

            float scaledLineWidthX = lineWidth / width;
            float scaledLineWidthY = lineWidth / height;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(scaledLineWidthX, scaledLineWidthY, 0),
                new Vector3(scaledLineWidthX, 1 - scaledLineWidthY, 0),
                new Vector3(1 - scaledLineWidthX, 1 - scaledLineWidthY, 0),
                new Vector3(1 - scaledLineWidthX, scaledLineWidthY, 0),

                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),
            };

            short[] indices = CreateHollowIndices(totalVertices, totalIndices);

            return new GeometryData(vertices, indices);
        }

        /// <summary>
        /// Creates a hollow circle given a specific radius and line width.
        /// </summary>
        /// <param name="radius">The desired radius of the circle. (In order for the line width to be accurate, it must be scaled by the desired radius).</param>
        /// <param name="lineWidth">The desired line width of the circle.</param>
        /// <returns>Generated shape data of your hollow circle.</returns>
        public static GeometryData CreateHollowCircle(float radius, float lineWidth)
        {
            return CreateHollowRegularPolygon(90, radius * 2, radius * 2, lineWidth);
        }

        internal static void UnloadContent()
        {
            foreach (GeometryData shapeData in shapes)
            {
                shapeData.Dispose();
            }
        }

        private static short[] CreateHollowIndices(int totalVertices, int totalIndices)
        {
            short[] indices = new short[totalIndices];
            int i = 0;
            int j = 0;
            for (; i < totalVertices / 2 - 1; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(i + (totalVertices / 2 + 1));
                indices[j + 2] = (short)(i + (totalVertices / 2));
                j += 3;
            }

            indices[j] = (short)(i);
            indices[j + 1] = (short)(totalVertices / 2);
            indices[j + 2] = (short)(i + (totalVertices / 2));
            j += 3;
            i++;

            indices[j] = (short)(i);
            indices[j + 1] = (short)(totalVertices / 2 - 1);
            indices[j + 2] = (short)(i - (totalVertices / 2));
            j += 3;
            i++;

            for (; i < totalVertices; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(i - (totalVertices / 2 + 1));
                indices[j + 2] = (short)(i - (totalVertices / 2));
                j += 3;
            }

            return indices;
        }

        private static void RegisterTriangle()
        {
            GeometryData shapeData = CreateRegularPolygon(3);
            shapeData.Managed = true;
            RegisterShapeData("Relatus_Triangle", shapeData);
        }

        private static void RegisterRightTriangle()
        {
            GeometryData rightTriangleData = new GeometryData
            (
                new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new short[]
                {
                    0, 1, 2,
                },
                true
            );

            RegisterShapeData("Relatus_RightTriangle", rightTriangleData);
        }

        private static void RegisterSquare()
        {
            GeometryData squareData = new GeometryData
            (
                new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new short[]
                {
                    0, 1, 2,
                    0, 2, 3
                },
                true
            );

            RegisterShapeData("Relatus_Square", squareData);
        }

        private static void RegisterCircle()
        {
            GeometryData shapeData = CreateRegularPolygon(90);
            shapeData.Managed = true;
            RegisterShapeData("Relatus_Circle", shapeData);
        }

        private static void RegisterStar()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<short> indices = new List<short>();
            int totalVertices = 10;
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;
            float angleIncrement = MathHelper.TwoPi / totalVertices;
            int alternate = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                if (alternate++ % 2 == 0)
                {
                    vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.25f, 0.5f + (float)Math.Sin(i) * 0.25f, 0));
                }
                else
                {
                    vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0));
                }

                if (vertices.Count >= totalVertices)
                    break;
            }

            int j = 1;
            for (int i = 0; i < totalIndices; i += 3)
            {
                indices.Add(0);
                indices.Add((short)(j + 1));
                indices.Add((short)j);
                j++;
            }

            RegisterShapeData("Relatus_Star", new GeometryData(vertices.ToArray(), indices.ToArray(), true));
        }
    }
}
