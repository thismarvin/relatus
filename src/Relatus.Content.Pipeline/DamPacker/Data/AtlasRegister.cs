using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker.Data
{
    class AtlasRegister
    {
        public string AtlasFileName { get; set; }
        public int Size { get; set; }
        public List<AtlasEntry> Sprites { get; set; }
    }
}
