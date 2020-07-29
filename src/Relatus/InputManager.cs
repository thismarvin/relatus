using Relatus.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// The current method of input.
    /// </summary>
    public enum InputMode
    {
        None,
        Keyboard,
        Controller
    }

    /// <summary>
    /// Maintains important input related logic, and provides functionality to accumulate a list of <see cref="InputProfile"/>'s
    /// </summary>
    public static class InputManager
    {
        public static InputMode InputMode { get; internal set; }

        private static readonly ResourceHandler<InputProfile> profiles;

        static InputManager()
        {
            InputMode = InputMode.None;
            profiles = new ResourceHandler<InputProfile>();
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

        internal static void Update()
        {
            KeyboardExt.Update();
            MouseExt.Update();
        }
    }
}
