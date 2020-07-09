using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    class CColor : IComponent
    {
        public Color Color { get; set; }

        public CColor()
        {
            Color = Color.White;
        }

        public CColor(Color color)
        {
            Color = color;
        }
    }
}
