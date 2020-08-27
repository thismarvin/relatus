using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System.Collections.Generic;

namespace Relatus
{
    internal enum StageType
    {
        Setup,
        Begin,
        End,
        Post,
    }

    public static class SketchManager
    {
        private static readonly GraphicsDevice graphicsDevice;
        private static readonly List<BetterSprite> layers;
        private static readonly Queue<Effect> effects;
        private static readonly List<RenderTarget2D> decommissioned;

        private static readonly List<StageType> completedStages;

        private static BatchExecution batchExecution;

        static SketchManager()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            layers = new List<BetterSprite>();
            effects = new Queue<Effect>();
            decommissioned = new List<RenderTarget2D>();

            completedStages = new List<StageType>();

            batchExecution = BatchExecution.DrawElements;
        }

        public static void SetBatchExecution(BatchExecution execution)
        {
            batchExecution = execution;
        }

        public static BetterSprite CreatePage(Texture2D texture)
        {
            float scale;
            if (texture.Width >= texture.Height)
            {
                scale = (float)WindowManager.WindowHeight / texture.Height;

                if (texture.Width * scale > WindowManager.WindowWidth)
                {
                    scale = (float)WindowManager.WindowWidth / texture.Width;
                }
            }
            else
            {
                scale = (float)WindowManager.WindowWidth / texture.Width;

                if (texture.Height * scale > WindowManager.WindowHeight)
                {
                    scale = (float)WindowManager.WindowHeight / texture.Height;
                }
            }

            float letterBox = (WindowManager.WindowWidth - texture.Width * scale) * 0.5f;
            float pillarBox = (WindowManager.WindowHeight - texture.Height * scale) * 0.5f;

            return new BetterSprite()
            {
                Texture = texture,
                Position = new Vector3(letterBox, -pillarBox, 0),
                Scale = new Vector3(scale, scale, 1)
            };
        }

        public static void Submit(BetterSprite sprite)
        {
            layers.Add(sprite);
        }

        ///// <summary>
        ///// Attches an <see cref="Effect"/> that is applied after every <see cref="Sketch"/> layer is drawn.
        ///// </summary>
        ///// <param name="effect"></param>
        public static void AttachEffect(Effect effect)
        {
            effects.Enqueue(effect);
        }

        internal static void RegisterStage(StageType stage)
        {
            if (!completedStages.Contains(stage))
                completedStages.Add(stage);
        }

        internal static bool VerifyQueue(params StageType[] expectedOrder)
        {
            if (expectedOrder.Length != completedStages.Count)
                return false;

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                if (completedStages[i] != expectedOrder[i])
                {
                    return false;
                }
            }

            return true;
        }

        internal static void GiveUpControl()
        {
            completedStages.Clear();
        }

        internal static void AddSketch(RenderTarget2D renderTarget)
        {
            //renderTargets.Add(renderTarget);

            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        internal static void AddSketch(BetterSprite layer)
        {
            layers.Add(layer);

            layer.SetPosition(layer.X + WindowManager.PillarBox * WindowManager.Scale, layer.Y - WindowManager.LetterBox * WindowManager.Scale, 0);

            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        internal static void Decomission(RenderTarget2D renderTarget)
        {
            decommissioned.Add(renderTarget);
        }

        internal static void Draw()
        {
            float x = WindowManager.WindowWidth * 0.5f;
            float y = -WindowManager.WindowHeight * 0.5f;

            Camera camera = Camera.CreateOrthographic(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2)
                .SetPosition(x, y, 1)
                .SetTarget(x, y, 0);

            if (effects.Count == 0)
            {
                // Draw all the saved RenderTargets.
                Sketch.SpriteBatcher
                    .AttachCamera(camera)
                    .SetBatchExecution(batchExecution)
                    .Begin()
                        .AddRange(layers)
                    .End();
            }
            else
            {
                // We need flatten all the layers into a single layer.
                RenderTarget2D flatten = new RenderTarget2D(graphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

                graphicsDevice.SetRenderTarget(flatten);
                graphicsDevice.Clear(Color.Transparent);

                Sketch.SpriteBatcher
                    .AttachCamera(camera)
                    .SetBatchExecution(batchExecution)
                    .Begin()
                        .AddRange(layers)
                    .End();

                graphicsDevice.SetRenderTarget(null);

                // Now that we have a single texture to work with we can apply all of the effects to said texture.
                BetterSprite sprite = new BetterSprite()
                {
                    Texture = ApplyEffects(flatten, effects)
                };

                BetterSketch.DrawSprite(sprite, camera);

                flatten.Dispose();
            }

            // Clean up.
            for (int i = 0; i < decommissioned.Count; i++)
            {
                decommissioned[i].Dispose();
            }
            decommissioned.Clear();
            layers.Clear();
        }

        internal static Texture2D ApplyEffects(Texture2D texture, Queue<Effect> effects)
        {
            // If there are no effects then return a new sprite with the texture attached.
            if (effects.Count == 0)
            {
                return texture;
            }

            // We are going to have to create a new RenderTarget2D, and then draw the texture multiple times on said render target in order to apply every effect.

            float x = texture.Width * 0.5f;
            float y = -texture.Height * 0.5f;

            Camera camera =
                Camera.CreateOrthographic(texture.Width, texture.Height, 0.5f, 2)
                .SetPosition(x, y, 1)
                .SetTarget(x, y, 0);

            // In order to apply multiple effects, and avoid weird visual artifacts, we need to create a RenderTarget2D for each effect.
            RenderTarget2D[] renderTargets = new RenderTarget2D[effects.Count];
            for (int i = 0; i < renderTargets.Length; i++)
            {
                renderTargets[i] = new RenderTarget2D(graphicsDevice, texture.Width, texture.Height);
            }

            int totalEffects = effects.Count;

            for (int i = 0; i < totalEffects; i++)
            {
                graphicsDevice.SetRenderTarget(renderTargets[i]);
                graphicsDevice.Clear(Color.Transparent);

                BetterSprite sprite = new BetterSprite()
                {
                    // The first pass starts with the initial texture. Every pass after that will use the last render target (which means the effects will accumulate).
                    Texture = i == 0 ? texture : renderTargets[i - 1],
                    RenderOptions = new RenderOptions()
                    {
                        Effect = effects.Dequeue()
                    }
                };

                BetterSketch.DrawSprite(sprite, camera);

                graphicsDevice.SetRenderTarget(null);
            }

            // Dispose all render targets except for the last one.
            for (int i = 0; i < renderTargets.Length - 1; i++)
            {
                renderTargets[i].Dispose();
            }

            // We cannot dispose of the last render target just yet. Instead we are going to have to add it to a list, and deal with it later.
            Decomission(renderTargets[renderTargets.Length - 1]);

            // Now that all the effects were applied, just return a sprite with the last render target as the texture.
            return renderTargets[renderTargets.Length - 1];
        }
    }
}

