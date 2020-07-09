using Microsoft.Xna.Framework.Input;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Input
{
    /// <summary>
    /// An extension of the default <see cref="Keyboard"/> class that provides additional functionality to interface with your keyboard.
    /// </summary>
    static class MoreKeyboard
    {
        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;

        /// <summary>
        /// Returns whether or not the given key was just pressed.
        /// </summary>
        /// <param name="key">The keyboard key that should be tested.</param>
        /// <returns>Whether or not the given key was just pressed.</returns>
        public static bool Pressed(Keys key)
        {
            if (!previousKeyState.IsKeyDown(key) && currentKeyState.IsKeyDown(key))
            {
                InputManager.InputMode = InputMode.Keyboard;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the given key is currently being held down.
        /// </summary>
        /// <param name="key">The keyboard key that should be tested.</param>
        /// <returns>Whether or not the given key was just pressed.</returns>
        public static bool Pressing(Keys key)
        {
            if (currentKeyState.IsKeyDown(key))
            {
                InputManager.InputMode = InputMode.Keyboard;
                return true;
            }

            return false;
        }

        internal static void Update()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
        }
    }
}
