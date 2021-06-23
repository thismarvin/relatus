using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
{
    public class CPosition : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public CPosition()
        {

        }

        public CPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
