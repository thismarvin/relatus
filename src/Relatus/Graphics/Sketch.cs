using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Utilities;
using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public static class Sketch
    {
        public static SpriteBatcher SpriteBatcher { get; private set; }
        public static GeometryBatcher GeometryBatcher { get; private set; }

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Stack<Queue<Effect>> effects;
        private static readonly Stack<RenderTarget2D> idle;

        private static RenderTarget2D current;
        private static Texture2D result;

        private static Color clearColor;
        private static int width;
        private static int height;

        static Sketch()
        {
            SpriteBatcher = new SpriteBatcher();
            GeometryBatcher = new GeometryBatcher();

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
            Sketch.width = width;
            Sketch.height = height;
        }

        public static void Begin()
        {
            // Let's hope the end user dealt with this already.
            result = null;

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

        public static void End()
        {
            graphicsDevice.SetRenderTarget(null);

            if (idle.Count > 0)
            {
                /// If there are any nested layers then we have to do additional logic before we can submit anything.

                // Apply all of this layer's effects. (Note that CreateSprite must be called while the current RenderTarget is null).
                Sprite sprite = new Sprite()
                {
                    Texture = Sketchbook.ApplyEffects(current, effects.Pop())
                };

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
                        new OrthographicCamera()
                        .SetProjection(current.Width, current.Height, 0.5f, 2)
                        .SetPosition(x, y, 1);

                    collection.Add(sprite);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }

                // We cannot dispose of the current layer just yet. Instead we are going to have to add it to a list, and deal with it later.
                Sketchbook.Decomission(current);

                // Everything has been taken care of, so we can set the current layer back to the parent layer.
                current = previous;
            }
            else
            {
                result = Sketchbook.ApplyEffects(current, effects.Pop());

                // We cannot dispose of the current layer just yet. Instead we are going to have to add it to a list, and deal with it later.
                Sketchbook.Decomission(current);

                // Make sure to set the current layer to null in order to prevent any unwanted nesting!
                current = null;
            }
        }

        public static void Save(out Texture2D texture)
        {
            texture = result;
        }

        public static void DrawSprite(Sprite sprite, Camera camera)
        {
            SpriteElements spriteGroup = new SpriteElements(1, sprite.Texture, sprite.RenderOptions);
            spriteGroup.Add(sprite);
            spriteGroup.ApplyChanges();
            spriteGroup.Draw(camera);
        }

        public static void DrawSpriteCollection(BatchExecution batchExecution, uint batchSize, IEnumerable<Sprite> sprites, Camera camera)
        {
            using SpriteCollection collection = new SpriteCollection(batchExecution, batchSize, sprites);
            collection.Draw(camera);
        }

        public static void DrawText(float x, float y, string text, BMFont font, BMFontShader fontShader, Camera camera)
        {
            DrawSpriteCollection(BatchExecution.DrawElements, 1, ImText.Create(x, y, text, font, fontShader), camera);
        }

        public static void DrawPolygon(Polygon polygon, Camera camera)
        {
            PolygonElements polygonGroup = new PolygonElements(1, polygon.GeometryData, polygon.RenderOptions);
            polygonGroup.Add(polygon);
            polygonGroup.ApplyChanges();
            polygonGroup.Draw(camera);
        }

        public static void DrawPolygonCollection(BatchExecution batchExecution, uint batchSize, IEnumerable<Polygon> polygons, Camera camera)
        {
            using PolygonCollection collection = new PolygonCollection(batchExecution, batchSize, polygons);
            collection.Draw(camera);
        }

        public static void DrawLineSegment(float x1, float y1, float x2, float y2, float lineWidth, Camera camera)
        {
            DrawPolygon(ImLineSegment.Create(x1, y1, x2, y2, lineWidth), camera);
        }

        public static void DrawLineGraph(Vector2[] points, float lineWidth, Camera camera)
        {
            DrawPolygon(ImLineSegment.Create(points, lineWidth), camera);
        }
    }
}
