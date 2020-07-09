using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mimoso.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Core
{
    /// <summary>
    /// The current method of input.
    /// </summary>
    public enum InputMode
    {
        Keyboard,
        Controller
    }

    /// <summary>
    /// Maintains important input related logic, and provides functionality to accumulate a list of <see cref="InputProfile"/>'s
    /// </summary>
    static class InputManager
    {
        public static InputMode InputMode { get; internal set; }
        public static InputHandler BasicInputHandler { get; private set; }

        private static readonly ResourceHandler<InputProfile> profiles;

        static InputManager()
        {
            InputMode = InputMode.Keyboard;
            profiles = new ResourceHandler<InputProfile>();

            LoadProfiles();

            BasicInputHandler = new InputHandler(PlayerIndex.One);
            BasicInputHandler.LoadProfile("Basic");
        }

        #region Hangle Input Profiles
        /// <summary>
        /// Save an <see cref="InputProfile"/> for future reference.
        /// </summary>
        /// <param name="profile"> The input profile that should be saved.</param>
        public static void SaveProfile(InputProfile profile)
        {
            profiles.Register(profile.Name, profile);
        }

        /// <summary>
        /// Get an <see cref="InputProfile"/> that was previously saved.
        /// </summary>
        /// <param name="name">The name of the input profile that was previously saved.</param>
        /// <returns>The saved input profile with the given name.</returns>
        public static InputProfile GetProfile(string name)
        {
            return profiles.Get(name);
        }

        /// <summary>
        /// Remove a saved <see cref="InputProfile"/>.
        /// </summary>
        /// <param name="name">The name of the input profile that was previously saved.</param>
        public static void RemoveProfile(string name)
        {
            profiles.Remove(name);
        }
        #endregion

        private static void LoadProfiles()
        {
            SaveProfile(BasicInputProfile());
        }

        private static InputProfile BasicInputProfile()
        {
            InputProfile basic = new InputProfile("Basic");

            basic.CreateMapping(
                "Up",
                new Keys[] { Keys.W, Keys.Up },
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp, Buttons.RightThumbstickUp }
            );
            basic.CreateMapping(
                "Down",
                new Keys[] { Keys.S, Keys.Down },
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown, Buttons.RightThumbstickDown }
            );
            basic.CreateMapping(
                "Left",
                new Keys[] { Keys.A, Keys.Left },
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft, Buttons.RightThumbstickLeft }
            );
            basic.CreateMapping(
                "Right",
                new Keys[] { Keys.D, Keys.Right },
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight, Buttons.RightThumbstickRight }
            );
            basic.CreateMapping(
                "Start",
                new Keys[] { Keys.Enter },
                new Buttons[] { Buttons.Start }
            );

            return basic;
        }

        internal static void Update()
        {
            Input.MoreKeyboard.Update();
            Input.MoreMouse.Update();

            BasicInputHandler.Update();
        }
    }
}
