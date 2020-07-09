using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics
{
    static class Sketch
    {
        private static readonly SpriteBatch spriteBatch;
        private static readonly Queue<Effect> shaders;
        private static RenderTarget2D accumulation;
        private static RenderTarget2D result;
        private static Color clearColor;
        private static bool disableRelay;
        private static bool preventFumbledRelay;

        static Sketch()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
            shaders = new Queue<Effect>();
        }

        /// <summary>
        /// Creates a background layer that fills the entire screen with a specified color.
        /// </summary>
        /// <param name="spriteBatch">The default Game's SpriteBatch object.</param>
        /// <param name="color">The color used to fill the background layer.</param>
        public static void CreateBackgroundLayer(Color color)
        {
            PreventFumble();

            if (!SketchManager.VerifyQueue())
                throw new MimosoException("CreateBackgroundLayer(spriteBatch, color) must be independent of any other Sketch calls.", new MethodOrderException());

            // Initialize a RenderTarget2D to accumulate all spriteBatch draw calls.
            accumulation = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

            // Setup the GraphicsDevice with the new accumulation RenderTarget2D.
            spriteBatch.GraphicsDevice.SetRenderTarget(accumulation);
            spriteBatch.GraphicsDevice.Clear(color);

            // Reset the GraphicsDevice's RenderTarget.
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            // Relay the final RenderTarget2D to be drawn by the LayerManager.
            SketchManager.AddSketch(accumulation);
        }

        /// <summary>
        /// Sets the upcoming layer's background to a specified color.
        /// This method should be called before <see cref="Begin()"/>.
        /// </summary>
        /// <param name="color">The color used to fill the background of the upcoming layer.</param>
        public static void SetBackgroundColor(Color color)
        {
            PreventFumble();

            SketchManager.RegisterStage(StageType.Setup);

            // Make sure that this method is always called before Begin(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup))
                throw new MimosoException("SetBackgroundColor(color) must be called before Begin(spriteBatch).", new MethodOrderException());

            clearColor = color;
        }

        /// <summary>
        /// Attaches an effect that will be applied to the entire upcoming layer
        /// (refer to <see cref="EffectType"/> for all implementations of <see cref="FX"/> by Morro).
        /// More than one effect can be attached to a Sketch at a time.
        /// Effects are applied in the same order as they are attached.
        /// This method should be called before <see cref="Begin()"/>.
        /// </summary>
        /// <param name="fx">The shader that will be applied to the entire upcoming layer.</param>
        public static void AttachEffect(FX fx)
        {
            AttachEffect(fx.Effect);
        }

        /// <summary>
        /// Attaches an effect that will be applied to the entire upcoming layer.
        /// More than one effect can be attached to a Sketch at a time.
        /// Effects are applied in the same order as they are attached.
        /// This method should be called before <see cref="Begin()"/>.
        /// </summary>
        /// <param name="effect">The shader that will be applied to the entire upcoming layer.</param>
        public static void AttachEffect(Effect effect)
        {
            PreventFumble();

            SketchManager.RegisterStage(StageType.Setup);

            // Make sure that this method is always called before Begin(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup))
                throw new MimosoException("AttachEffect(effect) must be called before Begin(spriteBatch).", new MethodOrderException());

            shaders.Enqueue(effect);
        }

        /// <summary>
        /// This will disable the upcoming layer from being drawn.
        /// This is useful for any effects that require additional passes.
        /// Note that if this method is called, you must call <see cref="InterceptRelay"/> after <see cref="End()"/>.
        /// This method should be called before <see cref="Begin()"/>.
        /// </summary>
        public static void DisableRelay()
        {
            SketchManager.RegisterStage(StageType.Setup);

            // Make sure that this method is always called before Begin(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup))
                throw new MimosoException("AttachEffect(effect) must be called before Begin(spriteBatch).", new MethodOrderException());

            disableRelay = true;
        }

        /// <summary>
        /// Creates a layer to accumulate all upcoming spriteBatch calls.
        /// This method should be called before <see cref="End()"/>.
        /// </summary>
        public static void Begin()
        {
            PreventFumble();
            if (disableRelay)
                preventFumbledRelay = true;

            SketchManager.RegisterStage(StageType.Setup);
            SketchManager.RegisterStage(StageType.Begin);

            // Make sure that this method is always called before End(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup, StageType.Begin))
                throw new MimosoException("Begin(spriteBatch) must be called before End(spriteBatch).", new MethodOrderException());

            // Initialize a RenderTarget2D to accumulate all spriteBatch draw calls.
            accumulation = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

            // Setup the GraphicsDevice with the new accumulation RenderTarget2D.
            spriteBatch.GraphicsDevice.SetRenderTarget(accumulation);
            spriteBatch.GraphicsDevice.Clear(clearColor);
            clearColor = Color.Transparent;
        }

        /// <summary>
        /// Applies any attached effects to the current layer, and passes the layer to the SketchManager to be drawn.
        /// This method should be called after <see cref="Begin()"/>.
        /// </summary>
        public static void End()
        {
            SketchManager.RegisterStage(StageType.End);

            // Make sure that this method is always called after Begin(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup, StageType.Begin, StageType.End))
                throw new MimosoException("End(spriteBatch) must be called after Begin(spriteBatch).", new MethodOrderException());

            // Reset the GraphicsDevice's RenderTarget.
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            if (shaders.Count > 0)
            {
                // Create an array of RenderTarget2Ds to store the accumulation of each shader.
                RenderTarget2D[] renderTargets = new RenderTarget2D[shaders.Count];
                for (int i = 0; i < renderTargets.Length; i++)
                {
                    renderTargets[i] = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);
                }

                int totalShaders = shaders.Count;
                Effect shader;

                // Iterate through all the shaders in the queue.
                for (int i = 0; i < totalShaders; i++)
                {
                    shader = shaders.Dequeue();

                    // Setup the GraphicsDevice with the current RenderTarget2D.
                    spriteBatch.GraphicsDevice.SetRenderTarget(renderTargets[i]);
                    spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                    // Apply the shader.
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, shader, null);
                    {
                        spriteBatch.Draw(i == 0 ? accumulation : renderTargets[i - 1], Vector2.Zero, Color.White);
                    }
                    spriteBatch.End();

                    // Dispose of the current shader.
                    shader.Dispose();
                }

                // Initialize a RenderTarget2D to capture the result of all the shaders.
                result = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

                // Setup the GraphicsDevice with the result RenderTarget2D.
                spriteBatch.GraphicsDevice.SetRenderTarget(result);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    spriteBatch.Draw(renderTargets[renderTargets.Length - 1], Vector2.Zero, Color.White);
                }
                spriteBatch.End();

                // Dispose of all the unnecessary RenderTarget2Ds.
                for (int i = 0; i < renderTargets.Length; i++)
                {
                    renderTargets[i].Dispose();
                }
                accumulation.Dispose();

                // Reset the GraphicsDevice's RenderTarget.
                spriteBatch.GraphicsDevice.SetRenderTarget(null);

                // Relay the final RenderTarget2D to be drawn by the LayerManager.
                if (!disableRelay)
                    SketchManager.AddSketch(result);
            }
            else
            {
                // Relay the accumulation RenderTarget2D to be drawn by the LayerManager.
                if (!disableRelay)
                    SketchManager.AddSketch(accumulation);
                // Otherwise set the result as the current accumulation to be intercepted by the user.
                else
                    result = accumulation;
            }
        }

        /// <summary>
        /// Passes the entire layer to the user. The SketchManager is no longer managing the life cycle of the layer.
        /// Failure to properly manage the layer will result in a memory leak, among other problems.
        /// This method should be called after <see cref="End()"/>.
        /// </summary>
        /// <returns>Returns the entire layer that was just created.</returns>
        public static RenderTarget2D InterceptRelay()
        {
            SketchManager.RegisterStage(StageType.Post);

            // Make sure that this method is always called after End(spriteBatch).
            if (!SketchManager.VerifyQueue(StageType.Setup, StageType.Begin, StageType.End, StageType.Post))
                throw new MimosoException("InterceptRelay() must be called after End(spriteBatch).", new MethodOrderException());

            // Make sure that DisableRelay() was called.
            if (result == null)
                throw new MimosoException("There is nothing to intercept. Make sure to call DisableRelay() before calling Begin(spriteBatch)", new MethodOrderException());

            SketchManager.GiveUpControl();
            disableRelay = false;
            preventFumbledRelay = false;

            return result;
        }

        /// <summary>
        /// Make sure that the unmanaged layer is addressed by the user.
        /// </summary>
        private static void PreventFumble()
        {
            if (preventFumbledRelay)
                throw new MimosoException("DisableRelay() was called, but InterceptRelay() was never called.", new MethodOrderException());
        }
    }
}
