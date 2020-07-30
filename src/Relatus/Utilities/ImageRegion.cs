using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public struct ImageRegion
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ImageRegion(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
