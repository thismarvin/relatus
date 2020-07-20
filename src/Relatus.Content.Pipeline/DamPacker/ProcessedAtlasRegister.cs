using Relatus.Content.Pipeline.DamPacker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker
{
    class ProcessedAtlasRegister
    {
        public string Metadata { get; private set; }
        public string[] Entries { get; private set; }

        public ProcessedAtlasRegister(AtlasRegister source)
        {
            Metadata = $"{source.AtlasFileName},{source.Size}";
            Entries = GetFormatedEntries(source.Sprites);        
        }

        private string[] GetFormatedEntries(List<AtlasEntry> entries)
        {
            string[] result = new string[entries.Count];

            for (int i = 0; i < entries.Count; i++)
            {
                result[i] = $"{entries[i].Name},{entries[i].X},{entries[i].Y},{entries[i].Width},{entries[i].Height},{entries[i].TrimOffset.Item1},{entries[i].TrimOffset.Item2}";
            }

            return result;
        }
    }
}
