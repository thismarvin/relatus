using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    class Outline : FX
    {
        public Texture2D Texture { get; private set; }
        public Color Color { get; private set; }
        public float Size { get; private set; }

        public Outline(Texture2D texture) : this(texture, 1, Color.Black)
        {

        }

        public Outline(Texture2D texture, float size, Color color) : base("Outline")
        {
            Texture = texture;
            Size = size * WindowManager.Scale;
            Color = color;

            Initialize();
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;

            Initialize();
        }

        public void SetColor(Color color)
        {
            Color = color;

            Initialize();
        }

        public void SetSize(float size)
        {
            Size = size;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextureWidth"].SetValue((float)Texture.Width);
            Effect.Parameters["TextureHeight"].SetValue((float)Texture.Height);
            Effect.Parameters["Size"].SetValue(Size);
            Effect.Parameters["Color"].SetValue(Color.ToVector3());
            Effect.Parameters["Transparency"].SetValue(1 - (Color.A / 255f));
        }
    }
}