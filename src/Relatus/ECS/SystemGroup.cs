using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class SystemGroup
    {
        public MorroSystem[] Systems { get; private set; }

        private int systemIndex;

        public SystemGroup(int capacity)
        {
            Systems = new MorroSystem[capacity];
        }

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
            for (int i = 0; i < systemIndex; i++)
            {
                if (Systems[i].Enabled && Systems[i] is IUpdateableSystem system)
                {
                    system.Update();
                }
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
