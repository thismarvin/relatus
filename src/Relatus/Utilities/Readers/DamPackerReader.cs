using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities.Readers
{
    internal class DamPackerReader : ContentTypeReader<SpriteAtlas>
    {
        protected override SpriteAtlas Read(ContentReader input, SpriteAtlas existingInstance)
        {
            SpriteAtlas result = new SpriteAtlas();

            result.ParseMeta(input.ReadString());

            int totalEntries = input.ReadInt32();
            for (int i = 0; i < totalEntries; i++)
            {
                result.AddEntry(input.ReadString());
            }

            return result;
        }
    }
}
