using Microsoft.Xna.Framework;
using Mimoso.Core;
using Mimoso.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.ECS
{
    class SCollision : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;

        private SBinPartitioner binPartitioner;

        private readonly int queryBuffer;

        public SCollision(Scene scene, uint tasks, int targetFPS) : base(scene, tasks, targetFPS)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CBoxCollider));
            Depend(typeof(SPhysics), typeof(SBinPartitioner));

            queryBuffer = 4;
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];

            List<int> queryResult = binPartitioner.Query(CreateQueryBounds());

            if (queryResult.Count == 0)
                return;

            ShapeSchema collisionInformation = ShapeSchemaHelper.CreateShapeSchema(CBoxColliderHelper.ShapeData, position, dimension, transform);
            Shape shape = new Shape(collisionInformation);

            ShapeSchema theirCollisionInformation;
            Shape theirShape;
            CPosition theirPosition;
            CDimension theirDimension;
            CTransform theirTransform;

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (!scene.EntityContains(queryResult[i], typeof(CTransform), typeof(CBoxCollider)))
                    continue;

                if (entity == queryResult[i])
                    continue;

                theirPosition = (CPosition)positions[queryResult[i]];
                theirDimension = (CDimension)dimensions[queryResult[i]];
                theirTransform = (CTransform)transforms[queryResult[i]];

                theirCollisionInformation = ShapeSchemaHelper.CreateShapeSchema(CBoxColliderHelper.ShapeData, theirPosition, theirDimension, theirTransform);
                theirShape = new Shape(theirCollisionInformation);

                ProcessResolution(CollisionHelper.GetResolution(shape, theirShape));
            }

            RectangleF CreateQueryBounds()
            {
                return new RectangleF(position.X - queryBuffer, position.Y - queryBuffer, dimension.Width + queryBuffer * 2, dimension.Height + queryBuffer * 2);
            }

            void ProcessResolution(Vector2 resolution)
            {
                position.X += resolution.X;
                position.Y += resolution.Y;
            }
        }

        public override void Update()
        {
            if (binPartitioner == null)
            {
                binPartitioner = scene.GetSystem<SBinPartitioner>();
            }

            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();

            base.Update();
        }

        private class Shape : IShape
        {
            public RectangleF Bounds { get; set; }
            public Vector2[] Vertices { get; set; }
            public LineSegment[] Edges { get; set; }

            public Shape(ShapeSchema shapeScheme)
            {
                Vertices = shapeScheme.Vertices;
                Edges = shapeScheme.Edges;

                CreateBounds();
            }

            private void CreateBounds()
            {
                float xMin = VertexFinder(Vertices, "x", "minimum");
                float xMax = VertexFinder(Vertices, "x", "maximum");
                float yMin = VertexFinder(Vertices, "y", "minimum");
                float yMax = VertexFinder(Vertices, "y", "maximum");

                float width = xMax - xMin;
                float height = yMax - yMin;

                Bounds = new RectangleF(xMin, yMin, width, height);
            }

            private float VertexFinder(Vector2[] vertices, string dimension, string qualifier)
            {
                float result = GetValueOf(0);

                for (int i = 1; i < vertices.Length; i++)
                {
                    if (Valid(GetValueOf(i)))
                    {
                        result = GetValueOf(i);
                    }
                }
                return result;

                float GetValueOf(int index)
                {
                    string formatted = dimension.ToLowerInvariant();

                    if (formatted == "x")
                    {
                        return vertices[index].X;
                    }
                    else if (formatted == "y")
                    {
                        return vertices[index].Y;
                    }

                    throw new ArgumentException();
                }

                bool Valid(float value)
                {
                    string formatted = qualifier.ToLowerInvariant();

                    if (formatted == "minimum")
                    {
                        return value < result;
                    }
                    else if (formatted == "maximum")
                    {
                        return value > result;
                    }

                    throw new ArgumentException();
                }
            }
        }
    }
}
