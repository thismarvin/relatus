using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality to send an event.
    /// </summary>
    public interface IEventAnnouncer
    {
        EventHandler<EventArgs> Announcement { get; set; }
    }

    public static class IEventAnnouncerHelper
    {
        public static void AnnounceEvent(this IEventAnnouncer announcer)
        {
            announcer.Announcement?.Invoke(null, EventArgs.Empty);
        }
    }
}
