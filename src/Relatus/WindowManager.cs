using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Relatus
{
    /// <summary>
    /// Maintains anything and everything related to the <see cref="GameWindow"/>, and provides functionality to safely manipulate the window.
    /// </summary>
    public static class WindowManager
    {
        public static float FPS { get; private set; }
        public static bool Fullscreen { get; private set; }
        public static float LetterBox { get; private set; }
        public static float PillarBox { get; private set; }
        public static int PixelHeight { get; private set; }
        public static int PixelWidth { get; private set; }
        public static float Scale { get; private set; }
        public static bool WideScreenSupported { get; private set; }

        public static float AspectRatio => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;
        /// <summary>
        /// The width of the entire display/screen.
        /// </summary>
        public static int DisplayWidth => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        /// <summary>
        /// The height of the entire display/screen.
        /// </summary>
        public static int DisplayHeight => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public static bool IsWideScreen => GraphicsAdapter.DefaultAdapter.IsWideScreen;
        public static DisplayOrientation Orientation => Engine.Instance.Window.CurrentOrientation;
        /// <summary>
        /// The current window height divided by the scale.
        /// </summary>
        public static int ScaledHeight => (int)Math.Ceiling(WindowHeight / Scale);
        /// <summary>
        /// The current window width divided by the scale.
        /// </summary>
        public static int ScaledWidth => (int)Math.Ceiling(WindowWidth / Scale);
        public static string Title => Engine.Instance.Window.Title;
        public static GameWindow Window => Engine.Instance.Window;
        /// <summary>
        /// The current height of the window.
        /// </summary>
        public static int WindowHeight => Engine.Graphics.PreferredBackBufferHeight;
        /// <summary>
        /// The current width of the window.
        /// </summary>
        public static int WindowWidth => Engine.Graphics.PreferredBackBufferWidth;

        private static readonly GeometryCollection polygonCollection;
        private static readonly Queue<float> sampleFPS;
        private static Polygon[] boxing;
        private static int defaultWindowWidth;
        private static int defaultWindowHeight;
        private static bool manipulatingScreen;

        public static EventHandler<EventArgs> WindowResize { get; set; }
        private static void RaiseWindowChangedEvent()
        {
            WindowResize?.Invoke(null, EventArgs.Empty);
        }

        static WindowManager()
        {
            sampleFPS = new Queue<float>();
            polygonCollection = new GeometryCollection(BatchExecution.DrawElements, 4);

            Engine.Instance.Window.ClientSizeChanged += HandleWindowResize;

            InitializeWindow();
            SetTitle("Relatus_Engine");

            /// When a MonoGame Windows Project application is fullscreen and the game's window has not been moved since startup,
            /// Microsoft.Xna.Framework.Input.Mouse.GetState().Y is offset unless the following line of code is included.
            Engine.Instance.Window.Position = Engine.Instance.Window.Position;
        }

        /// <summary>
        /// Sets the resolution of the game.
        /// The game will be scaled to maximize the area of the given pixel dimensions while also maintaining its aspect ratio.
        /// This means that regardless of the screen resolution, the game will always be displayed on the screen as optimally as possible.
        /// </summary>
        /// <param name="pixelWidth">The width of the resolution of your game in pixels.</param>
        /// <param name="pixelHeight">The height of the resolution of your game in pixels.</param>
        public static void SetGameResolution(int pixelWidth, int pixelHeight)
        {
            if (PixelWidth == pixelWidth && PixelHeight == pixelHeight)
                return;

            PixelWidth = pixelWidth;
            PixelHeight = pixelHeight;

            SetupOrientation();
            SetupBoxing();

            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();
        }

        /// <summary>
        /// Sets the default resolution of the window.
        /// </summary>
        /// <param name="windowWidth">The desired width of the window.</param>
        /// <param name="windowHeight">The desired height of the window.</param>
        public static void SetDefaultWindowResolution(int windowWidth, int windowHeight)
        {
            if (defaultWindowWidth == windowWidth && defaultWindowHeight == windowHeight)
                return;

#if __IOS__ || __ANDROID__
            return;
#else
            defaultWindowWidth = windowWidth;
            defaultWindowHeight = windowHeight;

            // Set Screen Dimensions.
            Engine.Graphics.PreferredBackBufferWidth = defaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = defaultWindowHeight;
#endif
            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();
        }

        /// <summary>
        /// Set the title of the window.
        /// </summary>
        /// <param name="title">The title of the window.</param>
        public static void SetTitle(string title)
        {
            Engine.Instance.Window.Title = title;
        }

        /// <summary>
        /// Sets whether or not VSync is enabled.
        /// </summary>
        /// <param name="enabled">Whether or not VSync should be enabled.</param>
        public static void EnableVSync(bool enabled)
        {
            Engine.Graphics.SynchronizeWithVerticalRetrace = enabled;
            Engine.Graphics.ApplyChanges();
        }

        /// <summary>
        /// Sets whether or not fullscreen is enabled.
        /// </summary>
        /// <param name="enabled">Whether or not fullscreen should be enabled.</param>
        public static void EnableFullscreen(bool enabled)
        {
            if (Fullscreen == enabled)
                return;

            Fullscreen = enabled;

            HandleFullscreenChange();
        }

        /// <summary>
        /// Sets whether or not wide screen support is enabled.
        /// </summary>
        /// <param name="enabled">Whether or not wide screen support is enabled.</param>
        public static void EnableWideScreenSupport(bool enabled)
        {
            WideScreenSupported = enabled;
        }

        public static (int, int, int, int) MaximizeSpace((int, int) region, float aspectRatio)
        {
            int width = 0;
            int height = 0;

            if (region.Item1 < region.Item2)
            {
                width = region.Item1;
                height = (int)Math.Round(width / aspectRatio);
            }
            else
            {
                height = region.Item2;
                width = (int)Math.Round(height * aspectRatio);
            }

            int pillar = (region.Item1 - width) / 2;
            int letter = (region.Item2 - height) / 2;

            return (width, height, pillar, letter);
        }

        private static void InitializeWindow()
        {
            PixelWidth = 320;
            PixelHeight = 180;

            defaultWindowWidth = PixelWidth * 2;
            defaultWindowHeight = PixelHeight * 2;

            Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            SetupOrientation();
            SetupBoxing();

            // Set Screen Dimensions.
#if __IOS__ || __ANDROID__
            Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
            Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
#else
            Engine.Graphics.PreferredBackBufferWidth = defaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = defaultWindowHeight;
#endif
            Engine.Instance.IsFixedTimeStep = false;
            Engine.Graphics.SynchronizeWithVerticalRetrace = true;

            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();
        }

        private static void SetupOrientation()
        {
            if (PixelWidth > PixelHeight)
            {
                Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            }
            else
            {
                Engine.Graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;
            }
        }

        private static void SetupBoxing()
        {
            int buffer = 1000;

            boxing = new Polygon[]
            {
                // Letter boxing.
                new Quad(-PixelWidth * 0.5f - buffer, PixelHeight * 0.5f + buffer, PixelWidth + buffer * 2, buffer) { Tint = Color.Black },
                new Quad(-PixelWidth * 0.5f - buffer, -PixelHeight * 0.5f, PixelWidth + buffer * 2, buffer) { Tint = Color.Black },
                // Pillar boxing.
                new Quad(-PixelWidth * 0.5f - buffer, PixelHeight * 0.5f + buffer, buffer, PixelHeight + buffer * 2) { Tint = Color.Black },
                new Quad(PixelWidth * 0.5f, PixelHeight * 0.5f + buffer, buffer, PixelHeight + buffer * 2) { Tint = Color.Black }
            };

            polygonCollection.Dispose();

            polygonCollection
                .Clear()
                .AddRange(boxing)
                .ApplyChanges();
        }

        private static void CalculateScale()
        {
            Scale = 0;

            if (PixelWidth > PixelHeight)
            {
                Scale = (float)WindowHeight / PixelHeight;

                // Check if letter boxing is required.
                if (PixelWidth * Scale > WindowWidth)
                {
                    Scale = (float)WindowWidth / PixelWidth;
                }
            }
            else
            {
                Scale = (float)WindowWidth / PixelWidth;

                // Check if pillar boxing is required.
                if (PixelHeight * Scale > WindowHeight)
                {
                    Scale = (float)WindowHeight / PixelHeight;
                }
            }
        }

        private static void CalculateBoxing()
        {
            LetterBox = (WindowHeight / Scale - PixelHeight) * 0.5f;
            PillarBox = (WindowWidth / Scale - PixelWidth) * 0.5f;
        }

        private static void ToggleFullScreen()
        {
            Fullscreen = !Fullscreen;

            HandleFullscreenChange();
        }

        private static void HandleResolutionChange()
        {
            CalculateScale();
            CalculateBoxing();

            RaiseWindowChangedEvent();
        }

        private static void HandleFullscreenChange()
        {
            if (Fullscreen)
            {
                Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
                Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
            }
            else
            {
                Engine.Graphics.PreferredBackBufferHeight = defaultWindowHeight;
                Engine.Graphics.PreferredBackBufferWidth = defaultWindowWidth;
            }

            Engine.Graphics.ToggleFullScreen();
            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();
        }

        private static void HandleWindowResize(object sender, EventArgs e)
        {
            /// It seems that, in a MonoGame DesktopGL project, changing the back buffer's dimensions raises a GameWindow.ClientSizeChanged event.
            /// This bool flag is needed in order to prevent an infinite loop of the two events raising each other.
            if (manipulatingScreen)
                return;

            manipulatingScreen = true;

            Engine.Graphics.PreferredBackBufferWidth = Math.Max(PixelWidth, Engine.Instance.Window.ClientBounds.Width);
            Engine.Graphics.PreferredBackBufferHeight = Math.Max(PixelHeight, Engine.Instance.Window.ClientBounds.Height);
            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();

            manipulatingScreen = false;
        }

        private static void UpdateFPS()
        {
            if (Engine.DeltaTime != 0)
            {
                sampleFPS.Enqueue(1 / Engine.DeltaTime);
            }

            if (sampleFPS.Count == 100)
            {
                FPS = sampleFPS.Average(i => i);
                sampleFPS.Dequeue();
            }
        }

        private static void UpdateInput()
        {
            if (Input.KeyboardExt.Pressed(Keys.F11))
            {
                ToggleFullScreen();
            }

#if !__IOS__ && !__TVOS__
            if (Input.KeyboardExt.Pressed(Keys.Escape))
            {
                Engine.Instance.Exit();
            }
#endif
        }

        internal static void Update()
        {
            UpdateInput();
            UpdateFPS();
        }

        internal static void Draw()
        {
            if (!WideScreenSupported)
            {
                //polygonCollection.Draw();
            }
        }
    }
}
