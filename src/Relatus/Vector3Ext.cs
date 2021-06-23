using Microsoft.Xna.Framework;
using System;

namespace Relatus
{
    public static class Vector3Ext
    {
        /// <summary>
        /// Converts spherical coordinates into Cartesian coordinates (represented as a Vector3).
        /// </summary>
        /// <param name="radius">The distance between the origin and the new point.</param>
        /// <param name="latitide"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Vector3 SphericalToCartesian(float radius, float latitide, float longitude)
        {
            latitide *= -1;
            latitide += MathHelper.PiOver2;

            return new Vector3(
                (float)(radius * Math.Sin(latitide) * Math.Sin(longitude)),
                (float)(radius * Math.Cos(latitide)),
                -(float)(radius * Math.Sin(latitide) * Math.Cos(longitude))
            );
        }

        public static bool AlmostEqual(this Vector3 self, Vector3 vector, float precision)
        {
            return
                MathExt.AlmostEqual(self.X, vector.X, precision) &&
                MathExt.AlmostEqual(self.Y, vector.Y, precision) &&
                MathExt.AlmostEqual(self.Z, vector.Z, precision);
        }
    }
}
