using Mimoso.Content.Pipeline.BMFonts.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimoso.Content.Pipeline.BMFonts
{
    class ProcessedBMFont
    {
        public string Info { get; private set; }
        public string Padding { get; private set; }
        public string Spacing { get; private set; }
        public List<string> Pages { get; private set; }
        public List<string> Characters { get; private set; }

        public ProcessedBMFont(BMFont source)
        {
            Pages = new List<string>();
            Characters = new List<string>();

            FormatInfo(source.Info);
            FormatPages(source.Pages);
            FormatCharacters(source.Chars);
        }

        private void FormatInfo(BMFontInfo info)
        {
            Info = string.Format
            (
                CultureInfo.InvariantCulture,
                "{0},{1},{2},{3}",
                info.Face,
                info.Size,
                info.Bold,
                info.Italic
            );

            Padding = info.Padding;
            Spacing = info.Spacing;
        }

        private void FormatPages(List<BMFontPage> pages)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                Pages.Add
                (
                    string.Format
                    (
                        CultureInfo.InvariantCulture,
                        "{0},{1}",
                        pages[i].ID,
                        pages[i].File
                    )
                );
            }
        }

        private void FormatCharacters(List<BMFontChar> characters)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                Characters.Add
                (
                    string.Format
                    (
                        CultureInfo.InvariantCulture,
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                        characters[i].ID,
                        characters[i].X,
                        characters[i].Y,
                        characters[i].Width,
                        characters[i].Height,
                        characters[i].XOffset,
                        characters[i].YOffset,
                        characters[i].XAdvance,
                        characters[i].Page
                    )
                );
            }
        }
    }
}
