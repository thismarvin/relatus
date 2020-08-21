using Microsoft.Xna.Framework;
using Relatus.Utilities;
using System;

namespace Relatus.Graphics
{
    public class Text : IDisposable
    {
        public string Content { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
        public Vector3 Translation { get; private set; }
        public Vector3 Scale { get; private set; }
        public Vector3 Rotation { get; private set; }

        public Vector3 Position => new Vector3(X, Y, Z);
        public Vector3 Center => new Vector3(X + Width * 0.5f * Scale.X, Y - Height * 0.5f * Scale.Y, Z);

        private readonly BMFont font;
        private readonly BMFontShader shader;

        private BetterSprite[] sprites;
        private int spriteIndex;
        private Matrix transform;

        private SpriteCollection spriteCollection;

        public Text(string content, BMFont font)
        {
            Content = content;
            this.font = font;

            Scale = Vector3.One;
            transform = Matrix.Identity;

            shader = new BMFontShader(Color.White, Color.Black, Color.Transparent);

            CreateText();
        }

        public Text SetPosition(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            return this;
        }

        public Text SetScale(float x, float y, float z)
        {
            Scale = new Vector3(x, y, z);

            return this;
        }

        public Text SetRotation(float roll, float pitch, float yaw)
        {
            Rotation = new Vector3(roll, pitch, yaw);

            return this;
        }

        public Text SetContent(string content)
        {
            if (Content == content)
                return this;

            Content = content;

            CreateText();

            return this;
        }

        public void SetStyling(Color textColor, Color outlineColor, Color aaColor)
        {
            shader
                .SetTextColor(textColor)
                .SetOutlineColor(outlineColor)
                .SetAAColor(aaColor);
        }

        private void CreateText()
        {
            if (Content.Length <= 0)
                return;

            sprites = new BetterSprite[Content.Length];
            spriteIndex = 0;

            float xOffset = X;

            for (int i = 0; i < Content.Length; i++)
            {
                char character = Content.Substring(i, 1).ToCharArray()[0];
                BMFontCharacter characterData = font.GetCharacterData(character);

                sprites[spriteIndex++] = new BetterSprite()
                    .SetPosition(xOffset + characterData.XOffset, Y - characterData.YOffset, Z)
                    .SetTexture(font.GetPage(characterData.Page))
                    .SetSampleRegion(characterData.ImageRegion)
                    .SetRenderOptions(new RenderOptions()
                    {
                        Effect = shader.Effect
                    });

                xOffset += characterData.XAdvance;
            }

            spriteCollection = new SpriteCollection(BatchExecution.DrawElements, (uint)Content.Length, sprites);

            Width = (float)Math.Ceiling(xOffset - X);
            Height = font.Size;
        }

        public Text ApplyChanges()
        {
            if (Content.Length <= 0)
                return this;

            float xOffset = X;

            transform =
                Matrix.CreateTranslation(-Center) *
                Matrix.CreateRotationZ(Rotation.Z) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.X) *
                Matrix.CreateTranslation(Center);

            for (int i = 0; i < Content.Length; i++)
            {
                char character = Content.Substring(i, 1).ToCharArray()[0];
                BMFontCharacter characterData = font.GetCharacterData(character);

                sprites[i]
                    .SetPosition(xOffset + characterData.XOffset * Scale.X, Y - characterData.YOffset * Scale.Y, Z)
                    .SetScale(Scale.X, Scale.Y, Scale.Z)
                    .SetRotation(Rotation.X, Rotation.Y, Rotation.Z);

                if (!Rotation.AlmostEqual(Vector3.Zero, 0.0001f))
                {
                    Vector3 result = Vector3.Transform(sprites[i].Position, transform);
                    sprites[i].SetPosition(result.X, result.Y, result.Z);
                }

                xOffset += characterData.XAdvance * Scale.X;
            }

            spriteCollection
                .Clear()
                .AddRange(sprites)
                .ApplyChanges();

            return this;
        }

        public void Draw(Camera camera)
        {
            spriteCollection.Draw(camera);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shader.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
