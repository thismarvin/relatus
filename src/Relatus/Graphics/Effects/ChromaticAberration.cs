using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    public class ChromaticAberration : FX
    {
        public Texture2D Texture { get; private set; }
        public Vector2 RedShift { get; private set; }
        public Vector2 GreenShift { get; private set; }
        public Vector2 BlueShift { get; private set; }
        public float Offset { get; private set; }

        public ChromaticAberration(Texture2D texture, float offset): this(texture, offset, new Vector2(-1, 0), Vector2.Zero, Vector2.Zero)
        {

        }

        public ChromaticAberration(Texture2D texture, float offset, Vector2 redShift, Vector2 greenShift, Vector2 blueShift) : base("ChromaticAberration")
        {
            Texture = texture;
            RedShift = redShift;
            GreenShift = greenShift;
            BlueShift = blueShift;
            Offset = offset * WindowManager.Scale;

            Initialize();
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;

            Initialize();
        }

        public void SetOffset(float offset)
        {
            Offset = offset;

            Initialize();
        }

        public void SetRedShift(Vector2 redShift)
        {
            RedShift = redShift;

            Initialize();
        }

        public void SetGreenShift(Vector2 greenShift)
        {
            GreenShift = greenShift;

            Initialize();
        }

        public void SetBlueShift(Vector2 blueShift)
        {
            BlueShift = blueShift;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextureWidth"].SetValue((float)Texture.Width);
            Effect.Parameters["TextureHeight"].SetValue((float)Texture.Height);
            Effect.Parameters["RedShift"].SetValue(RedShift);
            Effect.Parameters["GreenShift"].SetValue(GreenShift);
            Effect.Parameters["BlueShift"].SetValue(BlueShift);
            Effect.Parameters["Offset"].SetValue(Offset);
        }
    }
}
