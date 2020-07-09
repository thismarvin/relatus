using Microsoft.Xna.Framework.Content.Pipeline;
using Mimoso.Content.Pipeline.BMFonts.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mimoso.Content.Pipeline.BMFonts.Pipeline
{
  [ContentImporter(".fnt", DefaultProcessor = "BMFontProcessor", DisplayName = "BMFont Importer")]
  class BMFontImporter : ContentImporter<BMFont>
  {
    public override BMFont Import(string filename, ContentImporterContext context)
    {
      context.Logger.LogMessage("Importing XML file: {0}", filename);

      using (var streamReader = new StreamReader(filename))
      {
        var deserializer = new XmlSerializer(typeof(BMFont));
        return (BMFont)deserializer.Deserialize(streamReader);
      }
    }
  }
}
