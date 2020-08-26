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

        static SketchManager()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            layers = new List<BetterSprite>();
            effects = new Queue<Effect>();
            decommissioned = new List<RenderTarget2D>();

            completedStages = new List<StageType>();
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

            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        internal static void Decomission(RenderTarget2D renderTarget)
        {
            decommissioned.Add(renderTarget);
        }

        internal static void Draw()
        {
            float x = WindowManager.WindowWidth * 0.5f - WindowManager.PillarBox * WindowManager.Scale;
            float y = -WindowManager.WindowHeight * 0.5f + WindowManager.LetterBox * WindowManager.Scale;

            Camera camera = Camera.CreateOrthographic(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2)
                .SetPosition(x, y, 1)
                .SetTarget(x, y, 0);

            if (effects.Count == 0)
            {
                // Draw all the saved RenderTargets.
                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, (uint)layers.Count))
                {
                    collection.AddRange(layers);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }
            }
            else
            {
                // We need flatten all the layers into a single layer.
                RenderTarget2D flatten = new RenderTarget2D(graphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

                graphicsDevice.SetRenderTarget(flatten);
                graphicsDevice.Clear(Color.Transparent);

                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, (uint)layers.Count))
                {
                    collection.AddRange(layers);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }

                graphicsDevice.SetRenderTarget(null);

                // Now that we have a single texture to work with we can apply all of the effects to said texture.
                BetterSprite sprite = CreateSprite(flatten, effects);

                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, 1))
                {
                    collection.Add(sprite);
                    collection.ApplyChanges();
                    collection.Draw(camera);
                }

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

        internal static BetterSprite CreateSprite(Texture2D texture, Queue<Effect> effects)
        {
            // If there are no effects then return a new sprite with the texture attached.
            if (effects.Count == 0)
            {
                return new BetterSprite()
                {
                    Texture = texture
                };
            }

            // There is only one effect, so we can just return a sprite with the texture attached and then simply attach the effect to the sprite's render options.
            if (effects.Count == 1)
            {
                return new BetterSprite()
                {
                    Texture = texture,
                    RenderOptions = new RenderOptions()
                    {
                        Effect = effects.Dequeue()
                    }
                };
            }

            // There is more than one effect. We are going to have to create a new RenderTarget2D, and then draw the texture multiple times on said render target in order to apply every effect.

            float x = texture.Width * 0.5f;
            float y = -texture.Height * 0.5f;

            Camera camera =
                Camera.CreateOrthographic(texture.Width, texture.Height, 0.5f, 2)
                .SetPosition(x, y, 1)
                .SetTarget(x, y, 0);

            RenderTarget2D accumulation = new RenderTarget2D(graphicsDevice, texture.Width, texture.Height);

            graphicsDevice.SetRenderTarget(accumulation);
            graphicsDevice.Clear(Color.Transparent);

            int totalEffects = effects.Count;

            for (int i = 0; i < totalEffects; i++)
            {
                using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, 1))
                {
                    BetterSprite layer = new BetterSprite()
                    {
                        Texture = i == 0 ? texture : accumulation,
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

            // We cannot dispose of the accumulation just yet. Instead we are going to have to add it to a list, and deal with it later.
            Decomission(accumulation);

            // Now that all the effects were applied, just return a sprite with the accumulation as the texture.
            return new BetterSprite()
            {
                Texture = accumulation
            };
        }
    }
}

