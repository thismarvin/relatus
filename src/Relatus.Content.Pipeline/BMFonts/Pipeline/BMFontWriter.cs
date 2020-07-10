using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.Content.Pipeline.BMFonts.Pipeline
{
    [ContentTypeWriter]
    class BMFontWriter : ContentTypeWriter<ProcessedBMFont>
    {
        protected override void Write(ContentWriter output, ProcessedBMFont value)
        {
            output.Write(value.Info);

            int totalPages = value.Pages.Count;
            output.Write(totalPages);
            for (int i = 0; i < totalPages; i++)
            {
                output.Write(value.Pages[i]);
            }

            int totalCharacters = value.Characters.Count;
            output.Write(totalCharacters);
            for (int i = 0; i < totalCharacters; i++)
            {
                output.Write(value.Characters[i]);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Relatus.Utilities.Readers.BMFontReader, Relatus";
        }
    }
}
