using Microsoft.Xna.Framework;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// Provides all the functionality necessary to interface with an <see cref="InputProfile"/>.
    /// (Supports <see cref="MoreKeyboard"/> and <see cref="MoreGamePad"/> input types simultaneously).
    /// </summary>
    class InputHandler
    {
        public PlayerIndex PlayerIndex { get; private set; }

        private bool isBeingUpdated;
        private InputProfile inputProfile;
        private readonly MoreGamePad gamePad;

        /// <summary>
        /// Creates an instance of a <see cref="InputHandler"/> that is attached to a given <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        /// <param name="playerIndex">The player index this input handler will attach to.</param>
        public InputHandler(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            inputProfile = InputManager.GetProfile("Basic");
            gamePad = new MoreGamePad(PlayerIndex);
        }

        /// <summary>
        /// Load an <see cref="InputProfile"/> that has been registered with the <see cref="InputManager"/>.
        /// </summary>
        /// <param name="profile">The name of the input profile that was previously saved to the <see cref="InputManager"/>.</param>
        public void LoadProfile(string profile)
        {
            inputProfile = InputManager.GetProfile(profile);
        }

        /// <summary>
        /// Returns whether or not the given <see cref="InputMapping"/> was just activated.
        /// </summary>
        /// <param name="name">The name of the input mapping, that was saved within the current <see cref="InputProfile"/>, that should be tested.</param>
        /// <returns>Whether or not the <see cref="InputMapping"/> was just pressed.</returns>
        public bool Pressed(string name)
        {
            VerifyUpdateIsCalled();

            InputMapping inputMapping = inputProfile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                for (int i = 0; i < inputMapping.Keys.Length; i++)
                {
                    if (MoreKeyboard.Pressed(inputMapping.Keys[i]))
                    {
                        return true;
                    }
                }
            }

            if (gamePad.IsConnected)
            {
                for (int i = 0; i < inputMapping.Buttons.Length; i++)
                {
                    if (gamePad.Pressed(inputMapping.Buttons[i]))
                    {
                        return true;
                    }
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

            InputMapping inputMapping = inputProfile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                for (int i = 0; i < inputMapping.Keys.Length; i++)
                {
                    if (MoreKeyboard.Pressing(inputMapping.Keys[i]))
                    {
                        return true;
                    }
                }
            }

            if (gamePad.IsConnected)
            {
                for (int i = 0; i < inputMapping.Buttons.Length; i++)
                {
                    if (gamePad.Pressing(inputMapping.Buttons[i]))
                    {
                        return true;
                    }
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
        public void SetVibration(float leftMotorIntensity, float rightMotorIntensity, float duration)
        {
            gamePad.Vibrate(leftMotorIntensity, rightMotorIntensity, duration);
        }

        /// <summary>
        /// Resets the vibration of the gamepad to zero.
        /// </summary>
        public void ResetVibration()
        {
            gamePad.ResetVibration();
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
