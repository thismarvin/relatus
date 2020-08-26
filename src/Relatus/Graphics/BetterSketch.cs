using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public static class BetterSketch
    {
        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Stack<Queue<Effect>> effects;
        private static readonly Stack<RenderTarget2D> idle;
        private static readonly List<RenderTarget2D> expired;

        private static RenderTarget2D current;

        private static Color clearColor;
        private static int width;
        private static int height;

        static BetterSketch()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

            effects = new Stack<Queue<Effect>>();
            idle = new Stack<RenderTarget2D>();
            expired = new List<RenderTarget2D>();

            clearColor = Color.Transparent;
            width = WindowManager.WindowWidth;
            height = WindowManager.WindowHeight;
        }

        public static void SetClearColor(Color color)
        {
            clearColor = color;
        }

        public static void SetDimensions(int width, int height)
        {
            BetterSketch.width = width;
            BetterSketch.height = height;
        }

        public static void Begin()
        {
            if (current != null)
            {
                idle.Push(current);

                // Any nested layers should have the same dimensions as the parent layer.
                width = current.Width;
                height = current.Height;
            }

            effects.Push(new Queue<Effect>());
            current = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            graphicsDevice.SetRenderTarget(current);
            graphicsDevice.Clear(clearColor);

            // Reset properties to default.
            clearColor = Color.Transparent;
            width = (int)Math.Round(WindowManager.PixelWidth * WindowManager.Scale);
            height = (int)Math.Round(WindowManager.PixelHeight * WindowManager.Scale);
        }

        public static void AttachEffect(Effect effect)
        {
            effects.Peek().Enqueue(effect);
        }

        public static void End()
        {
            graphicsDevice.SetRenderTarget(null);

            if (idle.Count > 0)
            {
                RenderTarget2D previous = idle.Pop();

                graphicsDevice.SetRenderTarget(previous);

                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, 1))
                {
                    float x = current.Width * 0.5f;
                    float y = -current.Height * 0.5f;

                    Camera camera =
                        Camera.CreateOrthographic(current.Width, current.Height, 0.5f, 2)
                        .SetPosition(x, y, 1)
                        .SetTarget(x, y, 0);

                    BetterSprite sprite = CreateSprite(current, effects.Pop());

                    collection.Add(sprite);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }

                expired.Add(current);

                current = previous;
            }
            else
            {
                float scale;
                if (current.Width > current.Height)
                {
                    scale = (float)WindowManager.WindowHeight / current.Height;

                    // Check if letterboxing is required.
                    if (current.Width * scale > WindowManager.WindowWidth)
                    {
                        scale = (float)WindowManager.WindowWidth / current.Width;
                    }
                }
                else
                {
                    scale = (float)WindowManager.WindowWidth / current.Width;

                    // Check if letterboxing is required.
                    if (current.Height * scale > WindowManager.WindowHeight)
                    {
                        scale = (float)WindowManager.WindowHeight / current.Height;
                    }
                }

                BetterSprite layer = CreateSprite(current, effects.Pop());
                layer.Scale = new Vector3(scale, scale, 1);

                SketchManager.AddSketch(layer);

                expired.Add(current);
                current = null;
            }
        }

        internal static void Clean()
        {
            for (int i = 0; i < expired.Count; i++)
            {
                expired[i].Dispose();
            }
            expired.Clear();
        }

        private static BetterSprite CreateSprite(RenderTarget2D renderTarget, Queue<Effect> effects)
        {
            if (effects.Count == 0)
            {
                return new BetterSprite()
                {
                    Texture = renderTarget
                };
            }

            if (effects.Count == 1)
            {
                return new BetterSprite()
                {
                    Texture = renderTarget,
                    RenderOptions = new RenderOptions()
                    {
                        Effect = effects.Dequeue()
                    }
                };
            }

            float x = renderTarget.Width * 0.5f;
            float y = -renderTarget.Height * 0.5f;

            Camera camera =
                Camera.CreateOrthographic(renderTarget.Width, renderTarget.Height, 0.5f, 2)
                .SetPosition(x, y, 1)
                .SetTarget(x, y, 0);

            RenderTarget2D accumulation = new RenderTarget2D(graphicsDevice, renderTarget.Width, renderTarget.Height);

            graphicsDevice.SetRenderTarget(accumulation);
            graphicsDevice.Clear(Color.Transparent);

            int totalEffects = effects.Count;

            for (int i = 0; i < totalEffects; i++)
            {
                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, 1))
                {
                    BetterSprite layer = new BetterSprite()
                    {
                        Texture = i == 0 ? renderTarget : accumulation,
                        RenderOptions = new RenderOptions()
                        {
                            Effect = effects.Dequeue()
                        }
                    };

                    collection.Add(layer);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }
            }

            expired.Add(accumulation);

            graphicsDevice.SetRenderTarget(null);

            return new BetterSprite()
            {
                Texture = accumulation
            };
        }
    }
}
