using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mimoso.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Core
{
    public enum StageType
    {
        Setup,
        Begin,
        End,
        Post,
    }

    static class SketchManager
    {
        private static readonly SpriteBatch spriteBatch;
        private static readonly List<RenderTarget2D> renderTargets;
        private static readonly List<StageType> completedStages;
        private static Effect postProcessing;

        static SketchManager()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
            renderTargets = new List<RenderTarget2D>();
            completedStages = new List<StageType>();
        }

        /// <summary>
        /// Attches an <see cref="Effect"/> that is applied after every <see cref="Sketch"/> layer is drawn.
        /// </summary>
        /// <param name="postProcessing"></param>
        public static void AttachPostProcessingEffect(Effect postProcessing)
        {
            SketchManager.postProcessing = postProcessing;
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
            renderTargets.Add(renderTarget);

            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        internal static void Draw()
        {
            if (postProcessing != null)
            {
                // Initialize a RenderTarget2D to accumulate all RenderTargets.
                RenderTarget2D accumulation = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

                // Setup the GraphicsDevice with the new accumulation RenderTarget2D.
                spriteBatch.GraphicsDevice.SetRenderTarget(accumulation);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                // Draw all the saved RenderTargets onto the accumulation RenderTarget2D.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    for (int i = 0; i < renderTargets.Count; i++)
                    {
                        spriteBatch.Draw(renderTargets[i], Vector2.Zero, Color.White);
                    }
                }
                spriteBatch.End();

                // Reset the GraphicsDevice's RenderTarget.
                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                // Apply the shader.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, postProcessing, null);
                {
                    spriteBatch.Draw(accumulation, Vector2.Zero, Color.White);
                }
                spriteBatch.End();

                // Dispose of the accumulation RenderTarget.
                accumulation.Dispose();

                // Dispose of shader.
                postProcessing.Dispose();
                postProcessing = null;
            }
            else
            {
                // Draw all the saved RenderTargets.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    for (int i = 0; i < renderTargets.Count; i++)
                    {
                        spriteBatch.Draw(renderTargets[i], Vector2.Zero, Color.White);
                    }
                }
                spriteBatch.End();
            }

            // Dispose of all RenderTargets.
            for (int i = 0; i < renderTargets.Count; i++)
            {
                renderTargets[i].Dispose();
            }
            renderTargets.Clear();
        }
    }
}

