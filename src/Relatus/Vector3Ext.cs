using Microsoft.Xna.Framework;
using System;

namespace Relatus
{
    public static class Vector3Ext
    {
        public static Vector3 SphericalToCartesian(float radius, float inclination, float azimuth)
        {
            // Normally azimuth and inclination would be swapped in this formula.
            // However, because of OpenGL's coordinate system, they were switched around for the sake of convenience.
            return new Vector3(
                (float)(radius * Math.Sin(azimuth) * Math.Cos(inclination)),
                (float)(radius * Math.Sin(azimuth) * Math.Sin(inclination)),
                (float)(radius * Math.Cos(azimuth))
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
