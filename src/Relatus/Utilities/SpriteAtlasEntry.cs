using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public class SpriteAtlasEntry
    {
        public string Name { get; private set; }
        public ImageRegion ImageRegion { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        internal SpriteAtlasEntry(string name, int x, int y, int width, int height, int offsetX, int offsetY)
        {
            Name = name;
            ImageRegion = new ImageRegion(x, y, width, height);
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
