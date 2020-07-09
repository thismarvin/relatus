using Microsoft.Xna.Framework;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Maths
{
    interface IShape
    {
        Vector2[] Vertices { get; set; }
        LineSegment[] Edges { get; set; }
    }
}
