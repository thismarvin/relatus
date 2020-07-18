using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        public static Vector2 WindowLocation { get; private set; }
        public static RectangleF Bounds => new RectangleF(WindowLocation.X, WindowLocation.Y, 1, 1);
        public static int ScrollStride { get => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue; }

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;

        public static bool Pressing(params MouseButtons[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                switch (buttons[i])
                {
                    case MouseButtons.Left:
                        if (currentMouseState.LeftButton == ButtonState.Pressed)
                            return true;
                        break;

                    case MouseButtons.Right:
                        if (currentMouseState.RightButton == ButtonState.Pressed)
                            return true;
                        break;

                    case MouseButtons.Middle:
                        if (currentMouseState.MiddleButton == ButtonState.Pressed)
                            return true;
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
                            return true;
                        break;

                    case MouseButtons.Right:
                        if (previousMouseState.RightButton != ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
                            return true;
                        break;

                    case MouseButtons.Middle:
                        if (previousMouseState.MiddleButton != ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed)
                            return true;
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

            WindowLocation = new Vector2
            (
                currentMouseState.X / WindowManager.Scale - WindowManager.PillarBox,
                WindowManager.PixelHeight - (currentMouseState.Y / WindowManager.Scale - WindowManager.LetterBox)
            );
        }
    }
}
