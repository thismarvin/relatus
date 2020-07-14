using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// An extension of the default <see cref="Mouse"/> class that provides additional functionality to interface with your mouse.
    /// </summary>
    public static class MouseExt
    {
        public static RectangleF DynamicBounds { get; private set; }
        public static RectangleF StaticBounds { get; private set; }
        public static Vector2 SceneLocation { get => sceneLocation; }
        public static Vector2 WindowLocation { get => windowLocation; }
        public static int ScrollStride { get => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue; }

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;
        private static Vector2 sceneLocation;
        private static Vector2 windowLocation;

        public static bool Pressing(params MouseButtons[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                switch (buttons[i])
                {
                    case MouseButtons.Left:
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    case MouseButtons.Right:
                        if (currentMouseState.RightButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    case MouseButtons.Middle:
                        if (currentMouseState.MiddleButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public static bool Pressed(params MouseButtons[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                switch (buttons[i])
                {
                    case MouseButtons.Left:
                        if (previousMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    case MouseButtons.Right:
                        if (previousMouseState.RightButton != ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                    case MouseButtons.Middle:
                        if (previousMouseState.MiddleButton != ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed)
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the scroll wheel is being scrolled up.
        /// </summary>
        /// <returns>Whether or not the scroll wheel is being scrolled up.</returns>
        public static bool ScrollingUp()
        {
            return ScrollStride > 0;
        }

        /// <summary>
        /// Returns whether or not the scroll wheel is being scrolled down.
        /// </summary>
        /// <returns>Whether or not the scroll wheel is being scrolled down.</returns>
        public static bool ScrollingDown()
        {
            return ScrollStride < 0;
        }

        internal static void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (SceneManager.CurrentScene != null)
            {
                sceneLocation.X = currentMouseState.X / SceneManager.CurrentScene.Camera.Zoom + SceneManager.CurrentScene.Camera.TopLeft.X - WindowManager.PillarBox;
                sceneLocation.Y = currentMouseState.Y / SceneManager.CurrentScene.Camera.Zoom + SceneManager.CurrentScene.Camera.TopLeft.Y - WindowManager.LetterBox;
            }

            windowLocation.X = currentMouseState.X / CameraManager.Get(CameraType.Static).Zoom - WindowManager.PillarBox;
            windowLocation.Y = currentMouseState.Y / CameraManager.Get(CameraType.Static).Zoom - WindowManager.LetterBox;

            DynamicBounds = new RectangleF(sceneLocation.X, sceneLocation.Y, 1, 1);
            StaticBounds = new RectangleF(windowLocation.X, windowLocation.Y, 1, 1);
        }
    }
}
