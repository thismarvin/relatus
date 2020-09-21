using System;
using Microsoft.Xna.Framework.Input;
using Relatus.Graphics;
using Relatus.Utilities;

namespace Relatus
{
    /// <summary>
    /// Responsible for maintaining and displaying <see cref="DebugEntry"/>'s.
    /// (On PC, in-game debugging can be toggled by pressing F3).
    /// </summary>
    public static class DebugManager
    {
        public static bool Debugging { get; set; }
        public static bool ShowWireFrame { get; set; }

        private static readonly OrthographicCamera camera;
        private static readonly BMFont font;
        private static readonly BMFontShader fontShader;

        static DebugManager()
        {
            camera = new OrthographicCamera()
                .SetProjection(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2);
            camera.SetPosition(WindowManager.WindowWidth * 0.5f, -WindowManager.WindowHeight * 0.5f, 1);

            WindowManager.WindowResize += HandleResize;

            font = AssetManager.GetFont("Relatus_Probity");
            fontShader = new BMFontShader();
        }

        private static void HandleResize(object sender, EventArgs args)
        {
            camera.SetProjection(WindowManager.WindowWidth, WindowManager.WindowHeight, 0.5f, 2);
            camera.SetPosition(WindowManager.WindowWidth * 0.5f, -WindowManager.WindowHeight * 0.5f, 1);
        }

        private static void UpdateInput()
        {
            if (Input.KeyboardExt.Pressed(Keys.F3))
            {
                Debugging = !Debugging;
            }

            if (!Debugging)
                return;

            if (Input.KeyboardExt.Pressing(Keys.LeftShift) && Input.KeyboardExt.Pressed(Keys.D1))
            {
                ShowWireFrame = !ShowWireFrame;
            }
        }

        internal static void UnloadContent()
        {
            fontShader.Dispose();
        }

        internal static void Update()
        {
            UpdateInput();
        }

        internal static void Draw()
        {
            if (!Debugging)
                return;

            Sketch.SpriteBatcher
                .AttachCamera(camera)
                .SetBatchSize(1)
                .Begin()
                    .AddRange(ImText.Create(4, -4, $"{Math.Round(WindowManager.FPS)} FPS", font, fontShader))
                .End();
        }
    }
}
