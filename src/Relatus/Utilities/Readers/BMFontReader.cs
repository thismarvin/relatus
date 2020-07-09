using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities.Readers
{
    internal class BMFontReader : ContentTypeReader<BMFont>
    {
        protected override BMFont Read(ContentReader input, BMFont existingInstance)
        {
            BMFont result = new BMFont();

            result.ParseInfo(input.ReadString());

            int totalPages = input.ReadInt32();
            for (int i = 0; i < totalPages; i++)
            {
                result.AddPage(input.ReadString());
            }

            int totalCharacters = input.ReadInt32();
            for (int i = 0; i < totalCharacters; i++)
            {
                result.AddCharacter(input.ReadString());
            }

            return result;
        }
    }
}
