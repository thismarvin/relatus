using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality for a <see cref="MorroSystem"/> to update entities.
    /// </summary>
    public interface IUpdateableSystem : IMorroSystem
    {
        void Update();
    }
}
