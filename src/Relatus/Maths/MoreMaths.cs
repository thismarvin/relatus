using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{
    public static class MoreMaths
    {
        /// <summary>
        /// Given a number within a given range, remap said number to proportionally reflect a new range.
        /// </summary>
        /// <param name="current">The current value that will be remapped.</param>
        /// <param name="currentMin">The lower bound of the given range the current value is within.</param>
        /// <param name="currentMax">The upper bound of the given range the current value is within.</param>
        /// <param name="newMin">The lower bound of the new range the current value will be within.</param>
        /// <param name="newMax">The upper bound of the new range the current value will be within.</param>
        /// <returns></returns>
        public static double RemapRange(double current, double currentMin, double currentMax, double newMin, double newMax)
        {
            return newMin + (newMax - newMin) * (current - currentMin) / (currentMax - currentMin);
        }
    }
}
