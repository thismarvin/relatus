using Microsoft.Xna.Framework.Content.Pipeline;
using Relatus.Content.Pipeline.DamPacker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Content.Pipeline.DamPacker.Pipeline
{
    [ContentProcessor(DisplayName = "DamPacker Processor - Relatus")]
    class DamPackerProcessor : ContentProcessor<AtlasRegister, ProcessedAtlasRegister>
    {
        public override ProcessedAtlasRegister Process(AtlasRegister input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing Sprite Atlas");

            return new ProcessedAtlasRegister(input);
        }
    }
}
