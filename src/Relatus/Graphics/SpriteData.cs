using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct SpriteData
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string SpriteSheet { get; private set; }        

        public SpriteData(int x, int y, int width, int height, string spriteSheet)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            SpriteSheet = spriteSheet.ToLowerInvariant();
        }
    }
}
