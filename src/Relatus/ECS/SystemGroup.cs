using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.ECS
{
    public enum SystemExecutionType
    {
        /// <summary>
        /// Run all systems in a given group one after another in the order that they were added.
        /// </summary>
        Synchronous,
        /// <summary>
        /// Run all systems in a given group in parallel.
        /// </summary>
        Asynchronous
    }

    /// <summary>
    /// Stores an array of <see cref="MorroSystem"/>'s, and provides functionality to execute said systems.
    /// </summary>
    public class SystemGroup
    {
        public MorroSystem[] Systems { get; private set; }
        public SystemExecutionType Execution { get; set; }

        private int systemIndex;

        /// <summary>
        /// Creates an object which stores an array of <see cref="MorroSystem"/>'s, and provides functionality to execute said systems.
        /// </summary>
        /// <param name="capacity">The total amount of systems allowed in this group.</param>
        public SystemGroup(int capacity)
        {
            Systems = new MorroSystem[capacity];
            Execution = SystemExecutionType.Synchronous;
        }

        /// <summary>
        /// Sets the <see cref="SystemExecutionType"/> of the group.
        /// </summary>
        /// <param name="executionType">The execution type the system should follow.</param>
        /// <returns></returns>
        public SystemGroup SetExecutionType(SystemExecutionType executionType)
        {
            Execution = executionType;

            return this;
        }

        /// <summary>
        /// Adds a given <see cref="MorroSystem"/> to the group.
        /// </summary>
        /// <param name="systems">The systems to add the group.</param>
        /// <returns></returns>
        public SystemGroup Add(params MorroSystem[] systems)
        {
            if (systemIndex >= Systems.Length)
                return this;

            for (int i = 0; i < systems.Length; i++)
            {
                Systems[systemIndex++] = systems[i];
            }

            return this;
        }

        public void RunUpdateableSystems()
        {
            switch (Execution)
            {
                case SystemExecutionType.Synchronous:
                    for (int i = 0; i < systemIndex; i++)
                    {
                        if (Systems[i].Enabled && Systems[i] is IUpdateableSystem system)
                        {
                            system.Update();
                        }
                    }
                    break;

                case SystemExecutionType.Asynchronous:
                    Task.WaitAll(DivideSystemsIntoTasks());
                    break;
            }

            Task[] DivideSystemsIntoTasks()
            {
                int total = 0;
                for (int i = 0; i < Systems.Length; i++)
                {
                    total = Systems[i].Enabled && Systems[i] is IUpdateableSystem ? ++total : total;
                };

                Task[] tasks = new Task[total];
                int taskIndex = 0;

                for (int i = 0; i < Systems.Length; i++)
                {
                    if (Systems[i].Enabled && Systems[i] is IUpdateableSystem system)
                    {
                        tasks[taskIndex++] = Task.Run(() => system.Update());

                        if (taskIndex >= total)
                            break;
                    }
                }

                return tasks;
            }
        }

        public void RunDrawableSystems(Camera camera)
        {
            for (int i = 0; i < systemIndex; i++)
            {
                if (Systems[i].Enabled && Systems[i] is IDrawableSystem system)
                {
                    system.Draw(camera);
                }
            }
        }
    }
}
