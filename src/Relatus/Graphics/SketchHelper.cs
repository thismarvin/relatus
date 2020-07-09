using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
using Relatus.Graphics.Effects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    static class SketchHelper
    {

        private static readonly SpriteBatch spriteBatch;

        static SketchHelper()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public static void ApplyGaussianBlur(RenderTarget2D renderTarget, int passes)
        {
            for (int i = 0; i < passes * 2; i++)
                Sketch.AttachEffect(new Blur(Engine.RenderTarget, i % 2 == 0 ? new Vector2(1, 0) : new Vector2(0, 1), WindowManager.Scale * 0.75f));

            Sketch.Begin();
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                }
                spriteBatch.End();

                renderTarget.Dispose();
            }
            Sketch.End();
        }
    }
}
