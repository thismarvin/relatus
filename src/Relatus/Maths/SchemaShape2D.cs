using Microsoft.Xna.Framework;
using System;

namespace Relatus.Maths
{
    internal struct SchemaShape2D : IShape2D
    {
        public Vector2[] Vertices { get; set; }
        public LineSegment[] Edges { get; set; }

        public SchemaShape2D(Vector2[] vertices, LineSegment[] edges)
        {
            if (vertices.Length != edges.Length)
                throw new RelatusException("A 2D shape schema should have the same amount of vertices as edges.", new ArgumentException());

            Vertices = vertices;
            Edges = edges;
        }
    }
}
