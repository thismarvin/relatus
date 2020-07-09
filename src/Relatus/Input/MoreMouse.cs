using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Relatus.Core;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// An extension of the default <see cref="Mouse"/> class that provides additional functionality to interface with your mouse.
    /// </summary>
    static class MoreMouse
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

        /// <summary>
        /// Returns whether or not the left mouse button was just pressed.
        /// </summary>
        /// <returns>Whether or not the left mouse button was just pressed.</returns>
        public static bool PressedLeftClick()
        {
            return previousMouseState.LeftButton != ButtonState.Pressed && PressingLeftClick();
        }

        /// <summary>
        /// Returns whether or not the left mouse button is currently being held down.
        /// </summary>
        /// <returns>Whether or not the left mouse button is currently being held down.</returns>
        public static bool PressingLeftClick()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Returns whether or not the right mouse button was just pressed.
        /// </summary>
        /// <returns>Whether or not the right mouse button was just pressed.</returns>
        public static bool PressedRightClick()
        {
            return previousMouseState.RightButton != ButtonState.Pressed && PressingRightClick();
        }

        /// <summary>
        /// Returns whether or not the right mouse button is currently being held down.
        /// </summary>
        /// <returns>Whether or not the right mouse button is currently being held down.</returns>
        public static bool PressingRightClick()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Returns whether or not the middle mouse button was just pressed.
        /// </summary>
        /// <returns>Whether or not the middle mouse button was just pressed.</returns>
        public static bool PressedMiddleButton()
        {
            return previousMouseState.MiddleButton != ButtonState.Pressed && PressingMiddleButton();
        }

        /// <summary>
        /// Returns whether or not the middle mouse button is currently being held down.
        /// </summary>
        /// <returns>Whether or not the middle mouse button is currently being held down.</returns>
        public static bool PressingMiddleButton()
        {
            return currentMouseState.MiddleButton == ButtonState.Pressed;
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

            windowLocation.X = currentMouseState.X / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.PillarBox;
            windowLocation.Y = currentMouseState.Y / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.LetterBox;

            DynamicBounds = new RectangleF(sceneLocation.X, sceneLocation.Y, 1, 1);
            StaticBounds = new RectangleF(windowLocation.X, windowLocation.Y, 1, 1);
        }
    }
}
