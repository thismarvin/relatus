using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class AABB : Quad
    {
        public override Vector3 Rotation
        {
            get => base.Rotation;
            set => base.Rotation = Vector3.Zero;
        }

        public AABB(float x, float y, float width, float height) : base(x, y, width, height)
        {

        }
    }
}
