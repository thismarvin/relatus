using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Manages an instance of a <see cref="Random"/> class object, and adds additional random number generation related functionality.
    /// </summary>
    public static class RandomExt
    {
        /// <summary>
        /// The game's main pseudo-random number generator that is seeded upon startup.
        /// </summary>
        public static Random RNG { get; set; }

        static RandomExt()
        {
            RNG = new Random(DateTime.Now.Millisecond);
        }

        public static int Next(int lowerBound, int upperBound)
        {
            return RNG.Next(lowerBound, upperBound);
        }

        public static float NextFloat(float lowerBound, float upperBound)
        {
            return lowerBound + (float)RNG.NextDouble() * (upperBound - lowerBound);
        }

        public static double NextDouble(double lowerBound, double upperBound)
        {
            return lowerBound + RNG.NextDouble() * (upperBound - lowerBound);
        }

        /// <summary>
        /// Returns a boolean value that is consistent with a given probability.
        /// </summary>
        /// <param name="probability">Represents how likely the roll is to be successful. (The value should be greater than or equal to zero but less than or equal to one).</param>
        /// <returns>A boolean value that is consistent with a given probability.</returns>
        public static bool Roll(double probability)
        {
            if (probability <= 0)
                return false;

            if (probability >= 1)
                return true;

            return RNG.NextDouble() <= probability;
        }

        /// <summary>
        /// Returns a random double that depends on a gaussian (or normal) distribution.
        /// </summary>
        /// <param name="mean">The central value of your desired randomness.</param>
        /// <param name="standardDeviation">The amount of variance from the mean of your desired randomness.</param>
        /// <returns>A random double that depends on a gaussian (or normal) distribution.</returns>
        public static double Gaussian(double mean, double standardDeviation)
        {
            double u1 = 1.0 - RNG.NextDouble();
            double u2 = 1.0 - RNG.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return mean + standardDeviation * randStdNormal;
        }
    }
}
