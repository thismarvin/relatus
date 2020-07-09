using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    public class Blur : FX
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Direction { get; private set; }
        public float Offset { get; private set; }

        public Blur(Texture2D texture) : this(texture, new Vector2(1, 0), 1)
        {

        }

        public Blur(Texture2D texture, Vector2 direction, float offset) : base("Blur")
        {
            Texture = texture;
            Direction = direction;
            Offset = offset * WindowManager.Scale;

            Initialize();
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;

            Initialize();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextureWidth"].SetValue((float)Texture.Width);
            Effect.Parameters["TextureHeight"].SetValue((float)Texture.Height);
            Effect.Parameters["Direction"].SetValue(Direction);
            Effect.Parameters["Offset"].SetValue(Offset);
        }
    }
}
