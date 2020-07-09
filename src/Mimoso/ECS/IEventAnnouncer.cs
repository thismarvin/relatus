using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.ECS
{
    /// <summary>
    /// Provides functionality to send an event.
    /// </summary>
    interface IEventAnnouncer
    {
        EventHandler<EventArgs> Announcement { get; set; }
    }

    static class IEventAnnouncerHelper
    {
        public static void AnnounceEvent(this IEventAnnouncer announcer)
        {
            announcer.Announcement?.Invoke(null, EventArgs.Empty);
        }
    }
}
