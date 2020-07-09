using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality to toggle a system on and off.
    /// </summary>
    internal interface IMorroSystem
    {
        bool Enabled { get; set; }
    }
}
