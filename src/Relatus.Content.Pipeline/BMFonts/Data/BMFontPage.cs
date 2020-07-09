using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Relatus.Content.Pipeline.BMFonts.Data
{
    [Serializable]
    public class BMFontPage
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlAttribute("file")]
        public string File { get; set; }
    }
}
