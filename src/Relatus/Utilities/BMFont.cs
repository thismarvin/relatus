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
            Bold = data[2] == "1";
            Italic = data[3] == "1";
        }

        internal void AddPage(string page)
        {
            string[] data = page.Split(',');

            int id = int.Parse(data[0]);
            string file = data[1].Split('.')[0];

            if (!pages.ContainsKey(id))
            {
                pages.Add(id, file);
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
                characters.Add(id, new BMFontCharacter(id, x, y, width, height, xOffset, yOffset, xAdvance, pages[page]));
            }
        }

        internal BMFont LoadPages(string path)
        {
            // The BMFont's texture should be in the same directory as the .fnt file.
            // The path parameter is referencing said file, but in order to load the texture we need the path to the directory.
            // This following code is probably excessive, but it should get the path that we are looking for.
            string[] splitPath = path.Split('/');
            StringBuilder pathBuilder = new StringBuilder();
            for (int i = 0; i < splitPath.Length - 1; i++)
            {
                pathBuilder.Append($"{splitPath[i]}/");
            }
            string directory = pathBuilder.ToString();

            foreach (KeyValuePair<int, string> entry in pages)
            {
                string concatedPath = $"{directory}{entry.Value}";
                AssetManager.LoadImage(entry.Value, concatedPath);
            }

            return this;
        }

        public BMFontCharacter GetCharacterData(char character)
        {
            if (!characters.ContainsKey(character))
            {
                return new BMFontCharacter();
            }

            return characters[character];
        }
    }
}
