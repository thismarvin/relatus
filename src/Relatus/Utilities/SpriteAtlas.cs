using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    public class SpriteAtlas
    {
        public string Name { get; private set; }

        public List<SpriteAtlasEntry> Entries { get; private set; }

        public SpriteAtlas()
        {
            Entries = new List<SpriteAtlasEntry>();
        }

        internal void ParseMeta(string meta)
        {
            string[] data = meta.Split(',');
            Name = data[0];
        }

        internal void AddEntry(string entry)
        {
            string[] data = entry.Split(',');

            Entries.Add(new SpriteAtlasEntry(data[0], int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]), int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6])));
        }
    }
}
