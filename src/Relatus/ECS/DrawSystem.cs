using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An abstraction of a <see cref="MorroSystem"/> that can process <see cref="IComponent"/> data and preform draw logic every frame.
    /// </summary>
    public abstract class DrawSystem : MorroSystem, IDrawableSystem
    {
        public int Priority { get; set; }

        private readonly DrawSystemHandler drawSystemHandler;

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that will process <see cref="IComponent"/> data and preform draw logic every frame.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        internal DrawSystem(Scene scene) : base(scene)
        {
            drawSystemHandler = new DrawSystemHandler(this, DrawEntity);
        }

        public virtual void Draw(Camera camera)
        {
            drawSystemHandler.Draw(camera);
        }

        public abstract void DrawEntity(int entity, Camera camera);

        public int CompareTo(IDrawableSystem other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
