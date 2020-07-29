using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// Provides all the functionality necessary to interface with an <see cref="InputProfile"/>.
    /// (Supports <see cref="KeyboardExt"/>, <see cref="MouseExt"/>, and <see cref="SmartGamepad"/> input types simultaneously).
    /// </summary>
    public class InputHandler
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public InputProfile Profile { get; set; }

        private bool isBeingUpdated;
        private readonly SmartGamepad gamePad;

        /// <summary>
        /// Creates an instance of a <see cref="InputHandler"/> that is attached to a given <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        /// <param name="playerIndex">The player index this input handler will attach to.</param>
        /// <param name="profile">The input profile this input handler will use.</param>
        public InputHandler(PlayerIndex playerIndex, InputProfile profile)
        {
            PlayerIndex = playerIndex;
            Profile = profile;
            gamePad = new SmartGamepad(PlayerIndex);
        }

        /// <summary>
        /// Returns whether or not the given <see cref="InputMapping"/> was just activated.
        /// </summary>
        /// <param name="name">The name of the input mapping, that was saved within the current <see cref="InputProfile"/>, that should be tested.</param>
        /// <returns>Whether or not the <see cref="InputMapping"/> was just pressed.</returns>
        public bool Pressed(string name)
        {
            VerifyUpdateIsCalled();

            if (Profile == null)
                throw new RelatusException("The input profile has not been set.");

            InputMapping inputMapping = Profile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                if (KeyboardExt.Pressed(inputMapping.Keys) || MouseExt.Pressed(inputMapping.MouseButtons))
                {
                    return true;
                }
            }

            if (gamePad.IsConnected)
            {
                if (gamePad.Pressed(inputMapping.GamepadButtons))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the given <see cref="InputMapping"/> is currently being activated.
        /// </summary>
        /// <param name="name">The name of the input mapping, that was saved within the current <see cref="InputProfile"/>, that should be tested.</param>
        /// <returns>Whether or not the <see cref="InputMapping"/> was just pressed.</returns>
        public bool Pressing(string name)
        {
            VerifyUpdateIsCalled();

            if (Profile == null)
                throw new RelatusException("The input profile has not been set.");

            InputMapping inputMapping = Profile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                if (KeyboardExt.Pressing(inputMapping.Keys) || MouseExt.Pressing(inputMapping.MouseButtons))
                {
                    return true;
                }
            }

            if (gamePad.IsConnected)
            {
                if (gamePad.Pressing(inputMapping.GamepadButtons))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Vibrates the gamepad at a given intensity and for a given duration.
        /// </summary>
        /// <param name="leftMotorIntensity">The speed of the left motor, between 0.0 and 1.0.</param>
        /// <param name="rightMotorIntensity">The speed of the right motor, between 0.0 and 1.0.</param>
        /// <param name="duration">The duration of the vibration in milliseconds.</param>
        public InputHandler SetVibration(float leftMotorIntensity, float rightMotorIntensity, float duration)
        {
            gamePad.Vibrate(leftMotorIntensity, rightMotorIntensity, duration);

            return this;
        }

        /// <summary>
        /// Resets the vibration of the gamepad to zero.
        /// </summary>
        public InputHandler ResetVibration()
        {
            gamePad.ResetVibration();

            return this;
        }

        public void Update()
        {
            isBeingUpdated = true;
            gamePad.Update();
        }

        private void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
                throw new RelatusException("Make sure to call your InputHandler's Update() method before you use any of the built in methods.", new MethodExpectedException());
        }
    }
}
