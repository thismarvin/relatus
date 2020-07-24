using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker.Data
{
    class AtlasEntry
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ValueTuple<int, int> TrimOffset { get; set; }
    }
}
