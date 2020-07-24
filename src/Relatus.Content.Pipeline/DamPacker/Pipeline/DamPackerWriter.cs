using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker.Pipeline
{
    [ContentTypeWriter]
    class DamPackerWriter : ContentTypeWriter<ProcessedAtlasRegister>
    {
        protected override void Write(ContentWriter output, ProcessedAtlasRegister value)
        {
            output.Write(value.Metadata);

            int totalEntries = value.Entries.Length;
            output.Write(totalEntries);
            for (int i = 0; i < totalEntries; i++)
            {
                output.Write(value.Entries[i]);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Relatus.Utilities.Readers.DamPackerReader, Relatus";
        }
    }
}
