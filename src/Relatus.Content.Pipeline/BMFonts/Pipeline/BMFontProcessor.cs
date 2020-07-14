using Microsoft.Xna.Framework.Content.Pipeline;
using Relatus.Content.Pipeline.BMFonts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.Content.Pipeline.BMFonts.Pipeline
{
    [ContentProcessor(DisplayName = "BMFont Processor - Relatus")]
    class BMFontProcessor : ContentProcessor<BMFont, ProcessedBMFont>
    {
        public override ProcessedBMFont Process(BMFont input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing BMFont");

            return new ProcessedBMFont(input);
        }
    }
}
