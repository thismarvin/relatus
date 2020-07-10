using Microsoft.Xna.Framework;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    /// <summary>
    /// Counts down from a given duration. (Elapsed time is calculated using <see cref="GameTime"/>).
    /// </summary>
    public class Timer
    {
        public bool Enabled { get; set; }
        public float Duration { get; set; }
        public bool Done { get; private set; }
        public float ElapsedTime { get; private set; }

        /// <summary>
        /// Creates a timer of a given duration that is disabled by default.
        /// </summary>
        /// <param name="duration">The duration of the timer in milliseconds.</param>
        public Timer(float duration)
        {
            Duration = duration;
        }

        /// <summary>
        /// Enable the timer.
        /// </summary>
        public Timer Start()
        {
            Enabled = true;

            return this;
        }

        /// <summary>
        /// Disable the timer.
        /// </summary>
        public Timer Stop()
        {
            Enabled = false;

            return this;
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public Timer Reset()
        {
            ElapsedTime = 0;
            Done = false;

            return this;
        }

        public void Update()
        {
            if (Done || !Enabled)
                return;

            ElapsedTime += Engine.DeltaTime * 1000;
            Done = ElapsedTime >= Duration;
        }
    }
}
