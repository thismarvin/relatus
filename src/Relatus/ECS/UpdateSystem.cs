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
        /// <param name="factory">The factory this system will exist in.</param>
        public UpdateSystem(MorroFactory factory) : base(factory)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity);
        }

        /// <summary>
        /// Enables this system to update entities asynchronously by dividing entities into sections.
        /// </summary>
        /// <param name="sections">The total amount of sections to divide the entities into.</param>
        public virtual void EnableDivideAndConquer(uint sections)
        {
            updateSystemHandler.AsynchronousUpdateEnabled = true;
            updateSystemHandler.TotalTasks = sections;
        }

        public virtual void DisableDivideAndConquer()
        {
            updateSystemHandler.AsynchronousUpdateEnabled = false;
            updateSystemHandler.TotalTasks = 1;
        }

        /// <summary>
        /// Enables this systems to run at a fixed frame rate.
        /// </summary>
        /// <param name="updatesPerSecond">How often the system will update every second.</param>
        public virtual void EnableFixedUpdate(uint updatesPerSecond)
        {
            updateSystemHandler.FixedUpdateEnabled = true;
            updateSystemHandler.UpdatesPerSecond = updatesPerSecond;
        }

        public virtual void DisableFixedUpdate()
        {
            updateSystemHandler.FixedUpdateEnabled = false;
            updateSystemHandler.UpdatesPerSecond = 60;
        }

        public virtual void Update()
        {
            updateSystemHandler.Update();
        }

        public abstract void UpdateEntity(int entity);
    }
}
