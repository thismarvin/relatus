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
        private static readonly SpriteBatch spriteBatch;
        private static readonly List<BetterSprite> renderTargets;
        private static readonly List<StageType> completedStages;
        private static Effect postProcessing;

        static SketchManager()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            spriteBatch = GraphicsManager.SpriteBatch;
            renderTargets = new List<BetterSprite>();
            completedStages = new List<StageType>();
        }

        ///// <summary>
        ///// Attches an <see cref="Effect"/> that is applied after every <see cref="Sketch"/> layer is drawn.
        ///// </summary>
        ///// <param name="postProcessing"></param>
        //public static void AttachPostProcessingEffect(Effect postProcessing)
        //{
        //SketchManager.postProcessing = postProcessing;
        //}

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

        internal static void AddSketch(BetterSprite renderTarget)
        {
            renderTarget.SetPosition(-WindowManager.WindowWidth * 0.5f + WindowManager.PillarBox * WindowManager.Scale, WindowManager.WindowHeight * 0.5f - WindowManager.LetterBox * WindowManager.Scale, 0);

            renderTargets.Add(renderTarget);
            //
            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        internal static void Draw()
        {
            Camera camera = Camera.CreateOrthographic(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2);

            // Draw all the saved RenderTargets.
            Sketch.SpriteBatcher
                .AttachCamera(camera)
                .SetBatchSize((uint)renderTargets.Count)
                .Begin()
                    .AddRange(renderTargets)
                .End();

            // Dispose of all RenderTargets.
            for (int i = 0; i < renderTargets.Count; i++)
            {
                renderTargets[i].Texture.Dispose();
            }
            renderTargets.Clear();
        }
    }
}

