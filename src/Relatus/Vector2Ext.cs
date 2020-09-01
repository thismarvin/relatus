using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    public static class Vector2Ext
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
            float angle = (float)RandomExt.NextDouble(0, MathHelper.TwoPi);
            Vector2 result = Vector2Ext.FromAngle(angle);
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

        public static bool AlmostEqual(this Vector2 self, Vector2 vector, float precision)
        {
            return
                MathExt.AlmostEqual(self.X, vector.X, precision) &&
                MathExt.AlmostEqual(self.Y, vector.Y, precision);
        }
    }
}
