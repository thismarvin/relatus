using Microsoft.Xna.Framework.Input;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// A data structure that manages a collection of <see cref="InputMapping"/>.
    /// </summary>
    class InputProfile
    {
        public string Name { get; private set; }

        private readonly ResourceHandler<InputMapping> inputMappings;

        public InputProfile(string name)
        {
            Name = name;
            inputMappings = new ResourceHandler<InputMapping>();
        }

        #region Handle Input Mappings
        public void CreateMapping(string name, Keys[] keys)
        {
            CreateMapping(name, keys, new Buttons[0]);
        }

        /// <summary>
        /// Create an <see cref="InputMapping"/> that is associated with this input profile.
        /// </summary>
        /// <param name="name">The name the new input mapping should have.</param>
        /// <param name="keys">All the keys that should be associated with the new input mapping.</param>
        /// <param name="buttons">All the buttons that should be associated with the new input mapping.</param>
        public void CreateMapping(string name, Keys[] keys, Buttons[] buttons)
        {
            InputMapping inputMapping = new InputMapping(name, keys, buttons);
            inputMappings.Register(inputMapping.Name, inputMapping);
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

        public void Remap(string name, Keys[] keys)
        {
            Remap(name, keys, new Buttons[0]);
        }

        /// <summary>
        /// Remap an <see cref="InputMapping"/> that was previously created.
        /// </summary>
        /// <param name="name">The name that was given to the previously created input mapping.</param>
        /// <param name="keys">All the keys that should be associated with the new input mapping.</param>
        /// <param name="buttons">All the buttons that should be associated with the new input mapping.</param>
        public void Remap(string name, Keys[] keys, Buttons[] buttons)
        {
            GetMapping(name).Remap(keys, buttons);
        }

        /// <summary>
        /// Remove an already created <see cref="InputMapping"/>.
        /// </summary>
        /// <param name="name">The name that was given to the previously created input mapping.</param>
        public void RemoveMapping(string name)
        {
            inputMappings.Remove(name);
        }
        #endregion
    }
}
