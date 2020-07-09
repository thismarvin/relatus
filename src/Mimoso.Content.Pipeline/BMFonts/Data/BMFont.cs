using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mimoso.Content.Pipeline.BMFonts.Data
{
    [Serializable]
    [XmlRoot("font")]
    public class BMFont
    {
        [XmlElement("info")]
        public BMFontInfo Info { get; set; }

        [XmlElement("common")]
        public BMFontCommon Common { get; set; }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<BMFontPage> Pages { get; set; }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<BMFontChar> Chars { get; set; }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<BMFontKerning> Kernings { get; set; }
    }
}
