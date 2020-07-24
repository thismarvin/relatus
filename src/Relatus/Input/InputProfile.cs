using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// A data structure that manages a collection of <see cref="InputMapping"/>.
    /// </summary>
    public class InputProfile
    {
        public string Name { get; private set; }

        private readonly ResourceHandler<InputMapping> inputMappings;

        public InputProfile(string name)
        {
            Name = name;
            inputMappings = new ResourceHandler<InputMapping>();
        }

        #region Handle Input Mappings
        public InputProfile RegisterMapping(InputMapping mapping)
        {
            inputMappings.Register(mapping.Name, mapping);

            return this;
        }

        /// <summary>
        /// Get an <see cref="InputMapping"/> that was previously created.
        /// </summary>
        /// <param name="name">The name that was given to the previously created input mapping.</param>
        /// <returns>The input mapping with the given name.</returns>
        public InputMapping GetMapping(string name)
        {
            return inputMappings.Get(name);
        }

        public InputProfile RemapKeys(string name, Keys[] keys)
        {
            inputMappings.Get(name).Keys = keys;

            return this;
        }

        public InputProfile RemapGamepadButtons(string name, Buttons[] buttons)
        {
            inputMappings.Get(name).GamepadButtons = buttons;

            return this;
        }

        public InputProfile RemapMouseButtons(string name, MouseButtons[] mouseButtons)
        {
            inputMappings.Get(name).MouseButtons = mouseButtons;

            return this;
        }

        /// <summary>
        /// Remove an already created <see cref="InputMapping"/>.
        /// </summary>
        /// <param name="name">The name that was given to the previously created input mapping.</param>
        public InputProfile RemoveMapping(string name)
        {
            inputMappings.Remove(name);

            return this;
        }
        #endregion
    }
}
