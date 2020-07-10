using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality to listen to and handle a given event.
    /// </summary>
    public interface IEventListener
    {
        void HandleEvent(object sender, EventArgs e);
    }
}
