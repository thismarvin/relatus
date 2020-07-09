using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mimoso.Graphics;
using Mimoso.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mimoso.Core
{
    /// <summary>
    /// The current orientation of the window.
    /// </summary>
    public enum OrientationType
    {
        Landscape,
        Portrait
    }

    /// <summary>
    /// Maintains anything and everything related to the <see cref="GameWindow"/>, and provides functionality to safely manipulate the window.
    /// </summary>
    static class WindowManager
    {
        public static int PixelWidth { get; private set; }
        public static int PixelHeight { get; private set; }
        public static float Scale { get; private set; }
        public static RectangleF Bounds { get; private set; }

        public static int DisplayWidth { get; private set; }
        public static int DisplayHeight { get; private set; }
        public static int DefaultWindowWidth { get; private set; }
        public static int DefaultWindowHeight { get; private set; }
        public static int WindowWidth { get => Engine.Graphics.PreferredBackBufferWidth; }
        public static int WindowHeight { get => Engine.Graphics.PreferredBackBufferHeight; }
        public static RenderTarget2D RenderTarget { get; private set; }
        public static OrientationType Orientation { get; private set; }
        public static string Title { get; set; }
        public static bool Fullscreen { get; private set; }
        public static bool IsWideScreen { get; private set; }
        public static bool WideScreenSupported { get; private set; }

        public static float FPS { get; private set; }

        private static readonly Queue<float> sampleFPS;

        public static float LetterBox { get; private set; }
        public static float PillarBox { get; private set; }

        private static AABB[] boxing;
        private static readonly PolygonCollection polygonCollection;

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
            SetTitle("morroEngine");

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
            if (DefaultWindowWidth == windowWidth && DefaultWindowHeight == windowHeight)
                return;

#if __IOS__ || __ANDROID__
            return;
#else
            DefaultWindowWidth = windowWidth;
            DefaultWindowHeight = windowHeight;

            // Set Screen Dimensions.
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
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
            Title = title;
            Engine.Instance.Window.Title = Title;
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

            DefaultWindowWidth = PixelWidth * 2;
            DefaultWindowHeight = PixelHeight * 2;
            DisplayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            DisplayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Orientation = OrientationType.Landscape;
            Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            IsWideScreen = GraphicsAdapter.DefaultAdapter.IsWideScreen;
            SetupOrientation();
            SetupBoxing();

            // Set Screen Dimensions.
#if __IOS__ || __ANDROID__
            Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
            Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
#else
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
#endif
            Engine.Instance.IsFixedTimeStep = false;
            Engine.Graphics.SynchronizeWithVerticalRetrace = true;

            Engine.Graphics.ApplyChanges();

            HandleResolutionChange();
        }

        private static void SetupOrientation()
        {
            Orientation = PixelWidth > PixelHeight ? OrientationType.Landscape : OrientationType.Portrait;

            // Set Supported Orientations.
            switch (Orientation)
            {
                case OrientationType.Landscape:
                    Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
                    break;
                case OrientationType.Portrait:
                    Engine.Graphics.SupportedOrientations = DisplayOrientation.Portrait;
                    break;
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
                case OrientationType.Landscape:
                    Scale = (float)WindowHeight / PixelHeight;

                    // Check if letterboxing is required.
                    if (PixelWidth * Scale > WindowWidth)
                    {
                        Scale = (float)WindowWidth / PixelWidth;
                    }
                    break;

                case OrientationType.Portrait:
                    Scale = (float)WindowWidth / PixelWidth;

                    // Check if letterboxing is required.
                    if (PixelHeight * Scale > WindowHeight)
                    {
                        Scale = (float)WindowHeight / PixelHeight;
                    }
                    break;
            }

            Bounds = new RectangleF(0, 0, PixelWidth, PixelHeight);
        }

        private static void CalculateBoxing()
        {
            LetterBox = (WindowHeight / Scale - PixelHeight) / 2;
            PillarBox = (WindowWidth / Scale - PixelWidth) / 2;
        }

        private static void CalculateRenderTarget()
        {
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(Engine.Graphics.GraphicsDevice, WindowWidth, WindowHeight);
        }

        private static void HandleResolutionChange()
        {
            CalculateScale();
            CalculateBoxing();
            CalculateRenderTarget();

            RaiseWindowChangedEvent();
        }

        private static void ActivateFullScreen()
        {
            Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
            Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
        }

        private static void DeactivateFullScreen()
        {
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
        }

        private static void ToggleFullScreen()
        {
            Fullscreen = !Fullscreen;

            HandleFullscreenChange();
        }

        private static void HandleFullscreenChange()
        {
            if (Fullscreen)
            {
                ActivateFullScreen();
            }
            else
            {
                DeactivateFullScreen();
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

        private static void CalculateFPS()
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
            if (Input.MoreKeyboard.Pressed(Keys.F11))
            {
                ToggleFullScreen();
            }

#if !__IOS__ && !__TVOS__
            if (Input.MoreKeyboard.Pressed(Keys.Escape))
            {
                Engine.Instance.Exit();
            }
#endif
        }

        internal static void Update()
        {
            UpdateInput();
            CalculateFPS();
        }

        internal static void Draw()
        {
            if (!WideScreenSupported)
            {
                polygonCollection.Draw(CameraManager.GetCamera(CameraType.Static));
            }
        }
    }
}
