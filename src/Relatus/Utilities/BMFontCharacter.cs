using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public class BMFontCharacter
    {
        public int ID { get; private set; }
        public ImageRegion ImageRegion { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int XAdvance { get; private set; }
        public string Page { get; private set; }

        internal BMFontCharacter()
        {

        }

        internal BMFontCharacter(int id, int x, int y, int width, int height, int xOffset, int yOffset, int xAdvance, string page)
        {
            ID = id;
            ImageRegion = new ImageRegion(x, y, width, height);
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
            Page = page;
        }
    }
}
