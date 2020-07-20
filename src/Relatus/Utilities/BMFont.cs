using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Relatus.Utilities
{
    public class BMFont
    {
        public string FontFace { get; private set; }
        public int Size { get; private set; }
        public bool Bold { get; private set; }
        public bool Italic { get; private set; }

        private readonly Dictionary<int, string> pages;
        private readonly Dictionary<int, BMFontCharacter> characters;

        internal BMFont()
        {
            pages = new Dictionary<int, string>();
            characters = new Dictionary<int, BMFontCharacter>();
        }

        internal void ParseInfo(string info)
        {
            string[] data = info.Split(',');

            FontFace = data[0].ToString();
            Size = int.Parse(data[1]);
            Bold = data[2] == "1" ? true : false;
            Italic = data[3] == "1" ? true : false;
        }

        internal void AddPage(string page)
        {
            string[] data = page.Split(',');

            int id = int.Parse(data[0]);
            string file = data[1].Split('.')[0];

            if (!pages.ContainsKey(id))
            {
                pages.Add(id, file);

                AssetManager.LoadImage
                (
                    file,
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", "Assets/Fonts/", file)
                );
            }
        }

        internal void AddCharacter(string character)
        {
            string[] data = character.Split(',');

            int id = int.Parse(data[0]);
            int x = int.Parse(data[1]);
            int y = int.Parse(data[2]);
            int width = int.Parse(data[3]);
            int height = int.Parse(data[4]);
            int xOffset = int.Parse(data[5]);
            int yOffset = int.Parse(data[6]);
            int xAdvance = int.Parse(data[7]);
            int page = int.Parse(data[8]);

            if (!characters.ContainsKey(id))
            {
                characters.Add(id, new BMFontCharacter(id, xOffset, yOffset, xAdvance));

                SpriteManager.RegisterSpriteData
                (
                    string.Format(CultureInfo.InvariantCulture, "{0} {1}", FontFace, id),
                    x,
                    y,
                    width,
                    height,
                    pages[page]
                );
            }
        }

        public BMFontCharacter GetCharacterData(char character)
        {
            if (!characters.ContainsKey(character))
            {
                return new BMFontCharacter(0, 0, 0, Size);
            }

            return characters[character];
        }
    }
}
