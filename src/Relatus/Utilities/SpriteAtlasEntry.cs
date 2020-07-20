using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public class SpriteAtlasEntry
    {
        public string Name { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        internal SpriteAtlasEntry(string name, int x, int y, int width, int height, int offsetX, int offsetY)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
