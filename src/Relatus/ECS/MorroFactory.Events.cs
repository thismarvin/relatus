using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        /// <summary>
        /// Iterates through all registered <see cref="MorroSystem"/>'s and links each <see cref="IEventListener"/>
        /// to their respective <see cref="IEventAnnouncer"/> subscriptions.
        /// </summary>
        private void LinkSystems()
        {
            for (int i = 0; i < systemIndex; i++)
            {
                if (systems[i] is IEventAnnouncer announcer)
                {
                    for (int j = 0; j < systemIndex; j++)
                    {
                        if (i != j && systems[j] is IEventListener listener)
                        {
                            Type systemType = systems[i].GetType();

                            if (systems[j].Subscriptions.Contains(systemType))
                            {
                                announcer.Announcement += listener.HandleEvent;
                            }
                        }
                    }
                }
            }
        }

    }
}
