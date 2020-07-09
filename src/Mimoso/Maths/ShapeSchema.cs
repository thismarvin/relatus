using System;
using Microsoft.Xna.Framework;

namespace Mimoso.Maths
{
    class ShapeSchema : IShape
    {
        public Vector2[] Vertices { get; set; }
        public LineSegment[] Edges { get; set; }

        public ShapeSchema(Vector2[] vertices, LineSegment[] edges)
        {
            if (vertices.Length != edges.Length)
                throw new MimosoException("A shape schema should have the same amount of vertices as edges.", new ArgumentException());

            Vertices = vertices;
            Edges = edges;
        }

    }
}
