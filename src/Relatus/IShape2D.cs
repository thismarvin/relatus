using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    public interface IShape2D
    {
        Vector2[] Vertices { get; set; }
        LineSegment[] Edges { get; set; }
    }
}
