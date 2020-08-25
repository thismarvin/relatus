using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public static class BetterSketch
    {
        private static GraphicsDevice graphicsDevice;

        private static RenderTarget2D previous;
        private static RenderTarget2D current;

        private static Color clearColor;
        private static int width;
        private static int height;

        static BetterSketch()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

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
                previous = current;
            }

            current = new RenderTarget2D(graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            graphicsDevice.SetRenderTarget(current);
            graphicsDevice.Clear(clearColor);

            clearColor = Color.Transparent;
        }

        public static void End()
        {
            graphicsDevice.SetRenderTarget(null);

            if (previous != null)
            {
                graphicsDevice.SetRenderTarget(previous);

                Camera camera =
                    Camera.CreateOrthographic(width, height, 0.5f, 2)
                        .SetPosition(width * 0.5f, -height * 0.5f, 1)
                        .SetTarget(width * 0.5f, -height * 0.5f, 0);

                Sketch.SpriteBatcher
                    .AttachCamera(camera)
                    .SetBatchSize(1)
                    .Begin()
                        .Add(new BetterSprite()
                        {
                            Texture = current
                        })
                    .End();

                current = previous;
                previous = null;
            }
            else
            {
                float scale = 0;
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

                SketchManager.AddSketch(new BetterSprite() { Texture = current, Scale = new Vector3(scale, scale, 1) });

                //current.Dispose();
                //previous.Dispose();
                current = null;

                width = (int)Math.Round(WindowManager.PixelWidth * WindowManager.Scale);
                height = (int)Math.Round(WindowManager.PixelHeight * WindowManager.Scale);
            }
        }
    }
}
