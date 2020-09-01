using Microsoft.Xna.Framework;
using Relatus.Utilities;

namespace Relatus.Graphics
{
    public static class ImText
    {
        public static Sprite[] Create(float x, float y, string text, BMFont font, BMFontShader shader)
        {
            return Create(new Vector3(x, y, 0), text, font, shader);
        }

        private static Sprite[] Create(Vector3 position, string text, BMFont font, BMFontShader shader)
        {
            Sprite[] sprites = new Sprite[text.Length];

            float xOffset = position.X;

            for (int i = 0; i < text.Length; i++)
            {
                char character = text.Substring(i, 1).ToCharArray()[0];
                BMFontCharacter characterData = font.GetCharacterData(character);

                sprites[i] = Sprite.Create()
                    .SetTexture(font.GetPage(characterData.Page))
                    .SetSampleRegion(characterData.ImageRegion)
                    .SetRenderOptions(new RenderOptions()
                    {
                        Effect = shader.Effect
                    })
                .SetPosition(xOffset + characterData.XOffset, position.Y - characterData.YOffset, position.Z);

                xOffset += characterData.XAdvance;
            }

            return sprites;
        }
    }
}
