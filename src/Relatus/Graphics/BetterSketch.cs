using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Utilities;
using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public static class BetterSketch
    {
        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Stack<Queue<Effect>> effects;
        private static readonly Stack<RenderTarget2D> idle;

        private static RenderTarget2D current;
        private static Texture2D intercept;
        private static bool disableRelay;

        private static Color clearColor;
        private static int width;
        private static int height;

        static BetterSketch()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

            effects = new Stack<Queue<Effect>>();
            idle = new Stack<RenderTarget2D>();

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
                width = Math.Min(current.Width, width);
                height = Math.Min(current.Height, height);
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

        public static void DisableRelay()
        {
            disableRelay = true;
        }

        public static void End()
        {
            graphicsDevice.SetRenderTarget(null);

            if (idle.Count > 0)
            {
                /// If there are any nested layers then we have to do additional logic before we can submit anything.

                // Apply all of this layer's effects. (Note that CreateSprite must be called while the current RenderTarget is null).
                BetterSprite sprite = SketchManager.CreateSprite(current, effects.Pop());

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
                SketchManager.Decomission(current);

                // Everything has been taken care of, so we can set the current layer back to the parent layer.
                current = previous;
            }
            else
            {
                /// At this point we should be safe to start the submition process.

                // Apply all of this layer's effects. (Note that CreateSprite must be called while the current RenderTarget is null).
                BetterSprite layer = SketchManager.CreateSprite(current, effects.Pop());

                if (disableRelay)
                {
                    // If the user has disabled the relay to the SketchManager, then set the intercept to the current layer's sprite's texture.
                    intercept = layer.Texture;
                }
                else
                {
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
                }

                // We cannot dispose of the current layer just yet. Instead we are going to have to add it to a list, and deal with it later.
                SketchManager.Decomission(current);

                // Make sure to set the current layer to null in order to prevent any unwanted nesting!
                current = null;
            }
        }

        public static Texture2D InterceptRelay()
        {
            disableRelay = false;

            return intercept;
        }

        public static void DrawSprite(BetterSprite sprite, Camera camera)
        {
            SpriteElements spriteGroup = new SpriteElements(1, sprite.Texture, sprite.RenderOptions);
            spriteGroup.Add(sprite);
            spriteGroup.ApplyChanges();
            spriteGroup.Draw(camera);
        }

        public static void DrawSpriteCollection(BatchExecution batchExecution, uint batchSize, IEnumerable<BetterSprite> sprites, Camera camera)
        {
            using (SpriteCollection collection = new SpriteCollection(batchExecution, batchSize, sprites))
            {
                collection.Draw(camera);
            }
        }

        public static void DrawText(float x, float y, string text, BMFont font, BMFontShader fontShader, Camera camera)
        {
            DrawSpriteCollection(BatchExecution.DrawElements, 1, ImText.Create(x, y, text, font, fontShader), camera);
        }

        public static void DrawPolygon(Polygon polygon, Camera camera)
        {
            PolygonElements polygonGroup = new PolygonElements(1, polygon.Geometry, polygon.RenderOptions);
            polygonGroup.Add(polygon);
            polygonGroup.ApplyChanges();
            polygonGroup.Draw(camera);
        }

        public static void DrawPolygonCollection(BatchExecution batchExecution, uint batchSize, IEnumerable<Polygon> polygons, Camera camera)
        {
            using (PolygonCollection collection = new PolygonCollection(batchExecution, batchSize, polygons))
            {
                collection.Draw(camera);
            }
        }
    }
}
