using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality for a <see cref="MorroSystem"/> to draw entities.
    /// </summary>
    public interface IDrawableSystem : IMorroSystem, IComparable<IDrawableSystem>
    {
        int Priority { get; set; }

        void Draw(Camera camera);
    }
}
