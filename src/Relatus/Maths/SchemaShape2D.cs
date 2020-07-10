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
            Vertices = vertices;
            Edges = edges;
        }
    }
}
