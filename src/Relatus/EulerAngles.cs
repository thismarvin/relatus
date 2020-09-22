using System;
using Microsoft.Xna.Framework;

namespace Relatus
{
    /// <summary>
    /// Represents the orientation of an object in 3D space using three rotations around three distinct axes.
    /// </summary>
    public readonly struct EulerAngles
    {
        /// <summary>
        /// Rotation around the y-axis.
        /// </summary>
        public float Yaw { get; }
        /// <summary>
        /// Rotation around the x-axis.
        /// </summary>
        public float Pitch { get; }
        /// <summary>
        /// Rotation around the z-axis.
        /// </summary>
        public float Roll { get; }

        /// <summary>
        /// Creates a structure which represents the orientation of an object in 3D space using three rotations around three distinct axes.
        /// </summary>
        /// <param name="yaw">Rotation around the y-axis.</param>
        /// <param name="pitch">Rotation around the x-axis.</param>
        /// <param name="roll">Rotation around the z-axis.</param>
        public EulerAngles(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        /// <summary>
        /// Derives the respective Euler angles from a given <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="quaternion">The quaternion the Euler angles will be derived from.</param>
        /// <returns></returns>
        public static EulerAngles CreateFromQuaternion(Quaternion quaternion)
        {
            /// The follow has been yoinked from https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
            /// (Note that the XYZ values were modified to use OpenGL's coordinate system).

            // Calculate yaw.
            float siny_cosp = 2 * (quaternion.W * quaternion.Y + quaternion.Z * quaternion.X);
            float cosy_cosp = 1 - 2 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);

            float yaw = (float)Math.Atan2(siny_cosp, cosy_cosp);

            // Calculate pitch.
            float sinp = 2 * (quaternion.W * quaternion.X - quaternion.Y * quaternion.Z);
            float pitch;
            if (Math.Abs(sinp) >= 1)
            {
                // Use 90 degrees if out of range.
                pitch = (float)(Math.PI / 2 * Math.Sign(sinp));
            }
            else
            {
                pitch = (float)Math.Asin(sinp);
            }

            // Calculate roll.
            float sinr_cosp = 2 * (quaternion.W * quaternion.Z + quaternion.X * quaternion.Y);
            float cosr_cosp = 1 - 2 * (quaternion.Z * quaternion.Z + quaternion.X * quaternion.X);

            float roll = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            return new EulerAngles(yaw, pitch, roll);
        }
    }
}
