using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;
using Relatus.Content.Pipeline.DamPacker.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker.Pipeline
{
    [ContentImporter(".damp", DefaultProcessor = "DamPackerProcessor", DisplayName = "DamPacker Importer")]
    class DamPackerImporter : ContentImporter<AtlasRegister>
    {
        public override AtlasRegister Import(string filename, ContentImporterContext context)
        {
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(filename));

            var meta = data["METADATA"];
            string atlasFileName = meta.name;
            int size = (int)meta.size;

            List<AtlasEntry> entries = new List<AtlasEntry>();            
            foreach (var sprite in data["sprites"])
            {
                var trimRect = sprite["trimRect"];
                entries.Add
                (
                    new AtlasEntry()
                    {
                        Name = sprite.name,
                        X = (int)sprite.x,
                        Y = (int)sprite.y,
                        Width = (int)sprite.width,
                        Height = (int)sprite.height,
                        TrimOffset = ((int)trimRect.width, (int)sprite.height)
                    }
                );
            }

            return new AtlasRegister()
            {
                AtlasFileName = atlasFileName,
                Size = size,
                Sprites = entries
            };
        }
    }
}
