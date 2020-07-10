using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An abstraction of a <see cref="MorroSystem"/> that can process or manipulate <see cref="IComponent"/> data on every frame or a fixed basis.
    /// Note that an <see cref="UpdateSystem"/> is always executed before a <see cref="DrawSystem"/>.
    /// </summary>
    public abstract class UpdateSystem : MorroSystem, IUpdateableSystem
    {
        private readonly UpdateSystemHandler updateSystemHandler;

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that will process or manipulate <see cref="IComponent"/> data on every frame or a fixed basis.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        internal UpdateSystem(Scene scene, uint tasks) : base(scene)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity)
            {
                TotalTasks = tasks
            };
        }

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that will process or manipulate <see cref="IComponent"/> data on every frame or a fixed basis.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        /// <param name="targetFPS">The target framerate the system will update in.</param>
        internal UpdateSystem(Scene scene, uint tasks, int targetFPS) : base(scene)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity)
            {
                TotalTasks = tasks,
                TargetFPS = targetFPS
            };
        }

        public virtual void Update()
        {
            updateSystemHandler.Update();
        }

        public abstract void UpdateEntity(int entity);
    }
}
