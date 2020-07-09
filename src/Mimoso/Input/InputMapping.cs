using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Input
{
    /// <summary>
    /// A data structure that associates a name with a variety of <see cref="Microsoft.Xna.Framework.Input.Keys"/> and <see cref="Microsoft.Xna.Framework.Input.Buttons"/>.
    /// </summary>
    class InputMapping
    {
        public string Name { get; private set; }
        public Keys[] Keys { get; private set; }
        public Buttons[] Buttons { get; private set; }

        public InputMapping(string name, Keys[] keys) : this(name, keys, new Buttons[0])
        {

        }

        public InputMapping(string name, Keys[] keys, Buttons[] buttons)
        {
            Name = name;
            Keys = keys;
            Buttons = buttons;
        }

        public void Remap(Keys[] keys, Buttons[] buttons)
        {
            Keys = keys;
            Buttons = buttons;
        }
    }
}
