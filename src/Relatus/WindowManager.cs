using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Relatus.Graphics;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static float AspectRatio { get => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio; }
        /// <summary>
        /// The width of the entire display/screen.
        /// </summary>
        public static int DisplayWidth { get => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; }
        /// <summary>
        /// The height of the entire display/screen.
        /// </summary>
        public static int DisplayHeight { get => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height; }
        public static bool IsWideScreen { get => GraphicsAdapter.DefaultAdapter.IsWideScreen; }
        public static DisplayOrientation Orientation { get => Engine.Instance.Window.CurrentOrientation; }
        public static string Title { get => Engine.Instance.Window.Title; }
        public static GameWindow Window { get => Engine.Instance.Window; }
        /// <summary>
        /// The current height of the window.
        /// </summary>
        public static int WindowHeight { get => Engine.Graphics.PreferredBackBufferHeight; }
        /// <summary>
        /// The current width of the window.
        /// </summary>
        public static int WindowWidth { get => Engine.Graphics.PreferredBackBufferWidth; }

        private static readonly PolygonCollection polygonCollection;
        private static readonly Queue<float> sampleFPS;
        private static AABB[] boxing;
        private static int defaultWindowWidth;
        private static int defaultWindowHeight;
        private static bool manipulatingScreen;

        public static EventHandler<EventArgs> WindowChanged { get; set; }
        private static void RaiseWindowChangedEvent()
        {
            WindowChanged?.Invoke(null, EventArgs.Empty);
        }

        static WindowManager()
        {
            sampleFPS = new Queue<float>();
            polygonCollection = new PolygonCollection();

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

            boxing = new AABB[]
            {
                new AABB(-buffer, -buffer, PixelWidth + buffer * 2, buffer) { Color = Color.Black },
                new AABB(-buffer, PixelHeight,PixelWidth + buffer * 2, buffer) { Color = Color.Black },
                new AABB(-buffer, -buffer, buffer, PixelHeight + buffer * 2) { Color = Color.Black },
                new AABB(PixelWidth, -buffer, buffer, PixelHeight + buffer * 2) { Color = Color.Black }
            };

            polygonCollection.SetCollection(boxing);
        }

        private static void CalculateScale()
        {
            Scale = 0;

            switch (Orientation)
            {
                case DisplayOrientation.LandscapeRight:
                case DisplayOrientation.LandscapeLeft:
                    Scale = (float)WindowHeight / PixelHeight;

                    // Check if letterboxing is required.
                    if (PixelWidth * Scale > WindowWidth)
                    {
                        Scale = (float)WindowWidth / PixelWidth;
                    }
                    break;

                case DisplayOrientation.Portrait:
                case DisplayOrientation.PortraitDown:
                    Scale = (float)WindowWidth / PixelWidth;

                    // Check if letterboxing is required.
                    if (PixelHeight * Scale > WindowHeight)
                    {
                        Scale = (float)WindowHeight / PixelHeight;
                    }
                    break;
            }
        }

        private static void CalculateBoxing()
        {
            LetterBox = (WindowHeight / Scale - PixelHeight) / 2;
            PillarBox = (WindowWidth / Scale - PixelWidth) / 2;
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
                polygonCollection.Draw(CameraManager.Get(CameraType.Static));
            }
        }
    }
}
