using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    public static class MathExt
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

        /// <summary>
        /// Returns whether or not a given value is almost equal to a given target.
        /// </summary>
        /// <param name="value">The value that is being tested.</param>
        /// <param name="target">The target value that the previous parameter should equal.</param>
        /// <param name="precision">The margin of freedom used to determine whether or not the value equals the target.</param>
        /// <returns></returns>
        public static bool AlmostEqual(double value, double target, double precision)
        {
            return value - precision <= target && target <= value + precision;
        }

        public static Vector2 ConstrainPoint(double x, double y, RectangleF boundary)
        {
            double xConstrained = x;
            double yConstrained = y;

            xConstrained = Math.Max(boundary.Left, xConstrained);
            xConstrained = Math.Min(boundary.Right, xConstrained);

            yConstrained = Math.Max(boundary.Bottom, yConstrained);
            yConstrained = Math.Min(boundary.Top, yConstrained);

            return new Vector2((float)xConstrained, (float)yConstrained);
        }

        public static RectangleF ConstrainRectangle(RectangleF rectangle, RectangleF boundary)
        {
            Vector2 constrainedTopLeft = ConstrainPoint(rectangle.Left, rectangle.Top, boundary);
            Vector2 constrainedBottomRight = ConstrainPoint(rectangle.Right, rectangle.Bottom, boundary);

            float constrainedWidth = constrainedBottomRight.X - constrainedTopLeft.X;
            float constrainedHeight = constrainedTopLeft.Y - constrainedBottomRight.Y;

            return new RectangleF(constrainedTopLeft.X, constrainedTopLeft.Y, constrainedWidth, constrainedHeight);
        }
    }
}
