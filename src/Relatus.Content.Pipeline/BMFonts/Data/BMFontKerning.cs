using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Relatus.Content.Pipeline.BMFonts.Data
{
    [Serializable]
    public class BMFontKerning
    {
        [XmlAttribute("first")]
        public int First { get; set; }

        [XmlAttribute("second")]
        public int Second { get; set; }

        [XmlAttribute("amount")]
        public int Amount { get; set; }
    }
}
