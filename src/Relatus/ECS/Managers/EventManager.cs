using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Handles all functionality related to sending events between <see cref="IEventAnnouncer"/> and <see cref="IEventListener"/> capable <see cref="MorroSystem"/>'s.
    /// </summary>
    public class EventManager
    {
        private readonly SystemManager systemManager;

        public EventManager(SystemManager systemManager)
        {
            this.systemManager = systemManager;
        }

        /// <summary>
        /// Iterates through all registered <see cref="MorroSystem"/>'s and links each <see cref="IEventListener"/>
        /// to their respective <see cref="IEventAnnouncer"/> subscriptions.
        /// </summary>
        public void LinkSystems()
        {
            Type systemType;
            for (int i = 0; i < systemManager.TotalSystemsRegistered; i++)
            {
                if (systemManager.Systems[i] is IEventAnnouncer)
                {
                    for (int j = 0; j < systemManager.TotalSystemsRegistered; j++)
                    {
                        if (i != j && systemManager.Systems[j] is IEventListener)
                        {
                            systemType = systemManager.Systems[i].GetType();

                            if (systemManager.Systems[j].Subscriptions.Contains(systemType))
                            {
                                ((IEventAnnouncer)systemManager.Systems[i]).Announcement += ((IEventListener)systemManager.Systems[j]).HandleEvent;
                            }
                        }
                    }
                }
            }
        }
    }
}
