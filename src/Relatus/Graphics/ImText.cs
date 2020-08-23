using Microsoft.Xna.Framework;
using Relatus.Utilities;

namespace Relatus.Graphics
{
    public static class ImText
    {
        public static BetterSprite[] Create(float x, float y, string text, BMFont font)
        {
            BetterSprite[] sprites = new BetterSprite[text.Length];
            // ! This needs to be disposed somehow...
            BMFontShader shader = new BMFontShader(Color.White, Color.Black, Color.Transparent);

            float xOffset = x;

            for (int i = 0; i < text.Length; i++)
            {
                char character = text.Substring(i, 1).ToCharArray()[0];
                BMFontCharacter characterData = font.GetCharacterData(character);

                sprites[i] = BetterSprite.Create()
                    .SetTexture(font.GetPage(characterData.Page))
                    .SetSampleRegion(characterData.ImageRegion)
                    .SetRenderOptions(new RenderOptions()
                    {
                        Effect = shader.Effect
                    })
                    .SetPosition(xOffset + characterData.XOffset, y - characterData.YOffset, 0);

                xOffset += characterData.XAdvance;
            }

            return sprites;
        }
    }
}
