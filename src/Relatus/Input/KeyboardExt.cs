using Microsoft.Xna.Framework.Input;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// An extension of the default <see cref="Keyboard"/> class that provides additional functionality to interface with your keyboard.
    /// </summary>
    public static class KeyboardExt
    {
        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;

        /// <summary>
        /// Returns whether or not the given key is currently being held down.
        /// </summary>
        /// <param name="keys">The keyboard keys that should be tested.</param>
        /// <returns>Whether or not the given key was just pressed.</returns>
        public static bool Pressing(params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (currentKeyState.IsKeyDown(keys[i]))
                {
                    InputManager.InputMode = InputMode.Keyboard;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the given key was just pressed.
        /// </summary>
        /// <param name="key">The keyboard key that should be tested.</param>
        /// <returns>Whether or not the given key was just pressed.</returns>
        public static bool Pressed(params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (!previousKeyState.IsKeyDown(keys[i]) && currentKeyState.IsKeyDown(keys[i]))
                {
                    InputManager.InputMode = InputMode.Keyboard;
                    return true;
                }
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
