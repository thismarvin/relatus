using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public struct BMFontCharacter
    {
        public int ID { get; private set; }
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int XAdvance { get; private set; }

        internal BMFontCharacter(int id, int xOffset, int yOffset, int xAdvance)
        {
            ID = id;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }
    }
}
