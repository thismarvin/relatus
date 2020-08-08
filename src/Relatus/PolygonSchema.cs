using Microsoft.Xna.Framework;
using System;

namespace Relatus
{
    public class PolygonSchema 
    {
        public Vector2[] Vertices { get; set; }
        public LineSegment[] Edges { get; set; }

        public PolygonSchema(Vector2[] vertices, LineSegment[] edges)
        {
            Vertices = vertices;
            Edges = edges;
        }
    }
}
