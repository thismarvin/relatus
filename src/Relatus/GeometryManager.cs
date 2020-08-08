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
        private static readonly ResourceHandler<GeometryData> shapes;

        static GeometryManager()
        {
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
        /// Creates a filled regular polygon given a specific amount of vertices.
        /// </summary>
        /// <param name="totalVertices">The total amount of vertices the regular polygon will have.</param>
        /// <returns>Generated shape data of your regular polygon.</returns>
        public static GeometryData CreateRegularPolygon(int totalVertices)
        {
            if (totalVertices <= 2)
                throw new RelatusException("A polygon must have at least 3 vertices.", new ArgumentException());

            float angleIncrement = MathHelper.TwoPi / totalVertices;
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[totalVertices];
            short[] indices = new short[totalIndices];

            for (int i = 0; i < totalVertices; i++)
            {
                vertices[i] = new Vector3(0.5f + (float)Math.Cos(i * angleIncrement) * 0.5f, -0.5f + (float)Math.Sin(i * angleIncrement) * 0.5f, 0);
            }

            int j = 0;
            for (int i = 0; i < totalTriangles; i++)
            {
                indices[j++] = 0;
                indices[j++] = ((short)(i + 1));
                indices[j++] = ((short)(i + 2));
            }

            return new GeometryData(new Mesh(vertices, indices));
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

            float angleIncrement = MathHelper.TwoPi / initialTotalVertices;
            int actualTotalVertices = initialTotalVertices * 2;
            int totalTriangles = initialTotalVertices * 2;
            int totalIndices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[actualTotalVertices];

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

            for (int i = 0; i < initialTotalVertices; i++)
            {
                vertices[i] = new Vector3(0.5f + (float)Math.Cos(i * angleIncrement) * 0.5f, -0.5f + (float)Math.Sin(i * angleIncrement) * 0.5f, 0);
            }

            for (int i = 0; i < initialTotalVertices; i++)
            {
                vertices[initialTotalVertices + i] = new Vector3(0.5f + (float)Math.Cos(i * angleIncrement) * (0.5f - scaledLineWidthX), -0.5f + (float)Math.Sin(i * angleIncrement) * (0.5f - scaledLineWidthY), 0);
            }

            short[] indices = CreateHollowIndices(actualTotalVertices, totalIndices);

            return new GeometryData(new Mesh(vertices, indices));
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
            float scaledLineWidthX = lineWidth / width;
            float scaledLineWidthY = lineWidth / height;

            return new GeometryData
            (
                new Mesh
                (
                    new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, -1, 0),
                        new Vector3(1, -1, 0),
                        new Vector3(1, 0, 0),

                        new Vector3(scaledLineWidthX, -scaledLineWidthY, 0),
                        new Vector3(scaledLineWidthX, -1 + scaledLineWidthY, 0),
                        new Vector3(1 - scaledLineWidthX, -1 + scaledLineWidthY, 0),
                        new Vector3(1 - scaledLineWidthX, -scaledLineWidthY, 0),
                    },
                    new short[]
                    {
                        0, 1, 4,
                        1, 2, 5,
                        2, 3, 6,
                        3, 0, 7,
                        4, 1, 5,
                        5, 2, 6,
                        6, 3, 7,
                        7, 0, 4
                    }
                )
            );
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
            if (totalVertices % 2 != 0)
                throw new RelatusException("This algorithm will only work if the total amount of vertices is even.", new ArgumentException());

            short[] indices = new short[totalIndices];

            int totalInitialVertices = totalVertices / 2;
            int i = 0;
            int j = 0;

            for (; i < totalInitialVertices - 1; i++)
            {
                indices[j++] = (short)i;
                indices[j++] = (short)(i + 1);
                indices[j++] = (short)(totalInitialVertices + i);
            }

            indices[j++] = (short)i++;
            indices[j++] = 0;
            indices[j++] = (short)(totalVertices - 1);

            i = 0;
            for (; i < totalInitialVertices - 1; i++)
            {
                indices[j++] = (short)(totalInitialVertices + i);
                indices[j++] = (short)(i + 1);
                indices[j++] = (short)(totalInitialVertices + i + 1);
            }

            indices[j++] = (short)(totalVertices - 1);
            indices[j++] = 0;
            indices[j++] = (short)(totalInitialVertices);

            return indices;
        }

        private static void RegisterTriangle()
        {
            float cos30 = (float)Math.Sqrt(3) / 2;

            GeometryData shapeData = new GeometryData
            (
                new Mesh
                (
                    new Vector3[]
                    {
                        new Vector3(1 - cos30, 0, 0),
                        new Vector3(1 - cos30, -1, 0),
                        new Vector3(1, -0.5f, 0),
                    },
                    new short[]
                    {
                        0, 1, 2
                    }
                ),
                true
            );

            RegisterShapeData("Relatus_Triangle", shapeData);
        }

        private static void RegisterRightTriangle()
        {
            GeometryData rightTriangleData = new GeometryData
            (
                new Mesh(
                    new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, -1, 0),
                        new Vector3(1, -1, 0),
                    },
                    new short[]
                    {
                        0, 1, 2,
                    }
                ),
                true
            );

            RegisterShapeData("Relatus_RightTriangle", rightTriangleData);
        }

        private static void RegisterSquare()
        {
            GeometryData squareData = new GeometryData
            (
                new Mesh(
                    new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, -1, 0),
                        new Vector3(1, -1, 0),
                        new Vector3(1, 0, 0),
                    },
                    new short[]
                    {
                        0, 1, 2,
                        0, 2, 3
                    }
                ),
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
            int totalVertices = 10;
            float angleIncrement = MathHelper.TwoPi / totalVertices;
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[totalVertices];
            short[] indices = new short[totalIndices];

            int alternate = 0;

            for (int i = 0; i < totalVertices; i++)
            {
                if (alternate++ % 2 == 0)
                {
                    vertices[i] = new Vector3(0.5f + (float)Math.Cos(i * angleIncrement) * 0.25f, -0.5f + (float)Math.Sin(i * angleIncrement) * 0.25f, 0);
                }
                else
                {
                    vertices[i] = new Vector3(0.5f + (float)Math.Cos(i * angleIncrement) * 0.5f, -0.5f + (float)Math.Sin(i * angleIncrement) * 0.5f, 0);
                }
            }

            int j = 0;
            for (int i = 0; i < totalTriangles; i++)
            {
                indices[j++] = 0;
                indices[j++] = ((short)(i + 1));
                indices[j++] = ((short)(i + 2));
            }

            RegisterShapeData("Relatus_Star", new GeometryData(new Mesh(vertices, indices), true));
        }
    }
}
