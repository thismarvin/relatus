using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Maths
{
    static class VectorHelper
    {
        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return (float)Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
        }

        public static Vector2 PolarToCartesian(float radius, float theta)
        {
            return new Vector2
            (
                radius * (float)Math.Cos(theta),
                radius * (float)Math.Sin(theta)
            );
        }
        public static Vector2 FromAngle(float angle)
        {
            return PolarToCartesian(1, angle);
        }

        public static Vector2 Random()
        {
            return Random(1);
        }

        public static Vector2 Random(float magnitude)
        {
            float angle = (float)MoreRandom.NextDouble(0, MathHelper.TwoPi);
            Vector2 result = VectorHelper.FromAngle(angle);
            result.SetMagnitude(magnitude);

            return result;
        }

        public static void SetMagnitude(this ref Vector2 self, float magnitude)
        {
            self.Normalize();
            self *= magnitude;
        }

        public static void Limit(this ref Vector2 self, float maxLength)
        {
            if (self.LengthSquared() > maxLength * maxLength)
            {
                self.SetMagnitude(maxLength);
            }
        }

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
    }
}
