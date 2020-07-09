using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Input
{
    /// <summary>
    /// A data structure that associates a name with a variety of <see cref="Microsoft.Xna.Framework.Input.Keys"/> and <see cref="Microsoft.Xna.Framework.Input.Buttons"/>.
    /// </summary>
    public class InputMapping
    {
        public string Name { get; private set; }
        public Keys[] Keys { get; set; }
        public Buttons[] GamepadButtons { get; set; }
        public MouseButtons[] MouseButtons { get; set; }

        public InputMapping(string name)
        {
            Name = name;
            Keys = new Keys[0];
            GamepadButtons = new Buttons[0];
            MouseButtons = new MouseButtons[0];
        }
    }
}
