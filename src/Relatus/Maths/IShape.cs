using Microsoft.Xna.Framework;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{
    interface IShape
    {
        Vector2[] Vertices { get; set; }
        LineSegment[] Edges { get; set; }
    }
}
