using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An abstraction of a <see cref="MorroSystem"/> that combines the functionality of an <see cref="UpdateSystem"/> and a <see cref="DrawSystem"/>.
    /// </summary>
    public abstract class HybridSystem : MorroSystem, IUpdateableSystem, IDrawableSystem
    {
        public int Priority { get; set; }

        private readonly UpdateSystemHandler updateSystemHandler;
        private readonly DrawSystemHandler drawSystemHandler;

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that combines the functionality of an <see cref="UpdateSystem"/> and a <see cref="DrawSystem"/>.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        internal HybridSystem(Scene scene, uint tasks) : base(scene)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity)
            {
                TotalTasks = tasks,
            };

            drawSystemHandler = new DrawSystemHandler(this, DrawEntity);
        }

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that combines the functionality of an <see cref="UpdateSystem"/> and a <see cref="DrawSystem"/>.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        /// <param name="targetFPS">The target framerate the system will update in.</param>
        internal HybridSystem(Scene scene, uint tasks, int targetFPS) : base(scene)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity)
            {
                TotalTasks = tasks,
                TargetFPS = targetFPS
            };

            drawSystemHandler = new DrawSystemHandler(this, DrawEntity);
        }

        public virtual void Update()
        {
            updateSystemHandler.Update();
        }

        public virtual void Draw(Camera camera)
        {
            drawSystemHandler.Draw(camera);
        }

        public abstract void UpdateEntity(int entity);

        public abstract void DrawEntity(int entity, Camera camera);

        public int CompareTo(IDrawableSystem other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
