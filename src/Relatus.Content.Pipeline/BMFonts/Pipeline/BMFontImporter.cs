using Microsoft.Xna.Framework.Content.Pipeline;
using Relatus.Content.Pipeline.BMFonts.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Relatus.Content.Pipeline.BMFonts.Pipeline
{
    [ContentImporter(".fnt", DefaultProcessor = "BMFontProcessor", DisplayName = "BMFont Importer")]
    class BMFontImporter : ContentImporter<BMFont>
    {
        public override BMFont Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing XML file: {0}", filename);

            using StreamReader streamReader = new StreamReader(filename);
            XmlSerializer deserializer = new XmlSerializer(typeof(BMFont));

            return (BMFont)deserializer.Deserialize(streamReader);
        }
    }
}
