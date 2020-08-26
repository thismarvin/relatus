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
        private static readonly List<StageType> completedStages;
        private static readonly List<RenderTarget2D> decommissioned;

        //private static Effect postProcessing;

        static SketchManager()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            layers = new List<BetterSprite>();
            completedStages = new List<StageType>();
            decommissioned = new List<RenderTarget2D>();
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
            // Draw all the saved RenderTargets.
            using (SpriteCollection collection = new SpriteCollection(BatchExecution.DrawElements, (uint)layers.Count))
            {
                float x = WindowManager.WindowWidth * 0.5f - WindowManager.PillarBox * WindowManager.Scale;
                float y = -WindowManager.WindowHeight * 0.5f + WindowManager.LetterBox * WindowManager.Scale;

                Camera camera = Camera.CreateOrthographic(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2)
                    .SetPosition(x, y, 1)
                    .SetTarget(x, y, 0);

                collection.AddRange(layers);
                collection.ApplyChanges();
                collection.Draw(camera);
            }

            // Clean up.
            for (int i = 0; i < decommissioned.Count; i++)
            {
                decommissioned[i].Dispose();
            }
            decommissioned.Clear();
            layers.Clear();
        }
    }
}

