using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class CDimension : IComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public CDimension()
        {

        }

        public CDimension(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}
