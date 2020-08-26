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

                // The dimensions of any nested layer should not exceed the dimensions of the parent layer, but it is okay for the dimensions to be smaller.
                width = (int)Math.Min(current.Width, width);
                height = (int)Math.Min(current.Height, height);
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

            // If there are any nested layers then we have to do additional logic before we can submit anything.
            if (idle.Count > 0)
            {
                // Apply all of this layer's effects. (Note that CreateSprite must be called while the current RenderTarget is null).
                BetterSprite sprite = CreateSprite(current, effects.Pop());

                // Get the current layer's parent.
                RenderTarget2D previous = idle.Pop();

                // Set the current RenderTarget back to the parent layer.
                graphicsDevice.SetRenderTarget(previous);

                // Draw the current layer on top of its parent layer.
                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, 1))
                {
                    float x = current.Width * 0.5f;
                    float y = -current.Height * 0.5f;

                    Camera camera =
                        Camera.CreateOrthographic(current.Width, current.Height, 0.5f, 2)
                        .SetPosition(x, y, 1)
                        .SetTarget(x, y, 0);

                    collection.Add(sprite);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }

                // We cannot dispose of the current layer just yet. Instead we are going to have to add it to a list, and deal with it later.
                expired.Add(current);

                // Everything has been taken care of, so we can set the current layer back to the parent layer.
                current = previous;
            }
            // At this point we should be safe to start the submition process.
            else
            {
                // Apply all of this layer's effects. (Note that CreateSprite must be called while the current RenderTarget is null).
                BetterSprite layer = CreateSprite(current, effects.Pop());

                // Before we submit the finished product, we must first figure out what the layer must be scaled by to fit the entire screen.
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

                // Apply the scale to the sprite.
                layer.Scale = new Vector3(scale, scale, 1);

                // Everything should be setup, so we can finally submit the layer to the SketchManager.
                SketchManager.AddSketch(layer);

                // We cannot dispose of the current layer just yet. Instead we are going to have to add it to a list, and deal with it later.
                expired.Add(current);

                // Make sure to set the current layer to null in order to prevent any unwanted nesting!
                current = null;
            }
        }

        internal static void Clean()
        {
            // At this point all of the layers have finally been drawn, so we can dispose of them now.
            for (int i = 0; i < expired.Count; i++)
            {
                expired[i].Dispose();
            }
            expired.Clear();
        }

        private static BetterSprite CreateSprite(RenderTarget2D renderTarget, Queue<Effect> effects)
        {
            // If there are no effects then return a sprite with the renderTarget as the texture.
            if (effects.Count == 0)
            {
                return new BetterSprite()
                {
                    Texture = renderTarget
                };
            }

            // There is only one effect, so we can just return a sprite with the renderTarget as the texture and then simply attach the effect to the sprite's render options.
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

            // There is more than one effect, so we are going to have to draw the renderTarget multiple times in order to apply every effect.

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

            graphicsDevice.SetRenderTarget(null);

            expired.Add(accumulation);

            // Now that all the effects were applied, just return a sprite with the accumulation as the texture.
            return new BetterSprite()
            {
                Texture = accumulation
            };
        }
    }
}
