using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    struct HSV
    {
        public int H { get; private set; }
        public int S { get; private set; }
        public int V { get; private set; }

        public HSV(int h, int s, int v)
        {
            H = h;
            S = s;
            V = v;
        }

        public static HSV RGBtoHSV(Color color)
        {
            float max = color.R > color.G ? color.R : color.G;
            max = color.B > max ? color.B : max;
            float min = color.R < color.G ? color.R : color.G;
            min = color.B < min ? color.B : min;

            float delta = max - min;

            float h = 0;
            float s = 0;
            float v = max * 100 / 255;

            if (max > 0)
                s = delta / max * 100;
            else
                return new HSV((int)Math.Floor(h), (int)Math.Floor(s), (int)Math.Floor(v));

            if (max == color.R)
                h = 60 * (((color.G - color.B) / delta) % 6);
            else if (max == color.G)
                h = 60 * ((color.B - color.R) / delta + 2);
            else if (max == color.B)
                h = 60 * ((color.R - color.G) / delta + 4);

            return new HSV((int)Math.Floor(h), (int)Math.Floor(s), (int)Math.Floor(v));
        }

        public override string ToString()
        {
            return $"(H:{H}, S:{S}, V:{V})";
        }
    }
}
