using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class AABB : Quad
    {
        public override float Rotation { get => base.Rotation; set => base.Rotation = 0; }

        public AABB(float x, float y, float width, float height) : base(x, y, width, height)
        {

        }
    }
}
