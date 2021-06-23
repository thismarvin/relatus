using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Relatus
{
    public static class ColorExt
    {
        /// <summary>
        /// Returns a new <see cref="Color"/> from a given hexidecimal value.
        /// </summary>
        /// <param name="hex">The base sixteen value that represents the desired color.</param>
        /// <param name="alpha">The opacity of the color. (Note that the final color will be premultiplied by this value).</param>
        /// <returns>A new <see cref="Color"/> from a given hexidecimal value.</returns>
        public static Color CreateFromHex(uint hex, float alpha = 1)
        {
            int r = ValidateColorValue(((hex & 0xff0000) >> 16));
            int g = ValidateColorValue(((hex & 0xff00) >> 8));
            int b = ValidateColorValue((hex & 0xff));

            return new Color(r, g, b) * ValidateAlpha(alpha);
        }

        /// <summary>
        /// Returns a new <see cref="Color"/> from a given hexidecimal value.
        /// </summary>
        /// <param name="hex">The base sixteen value that represents the desired color.</param>
        /// <param name="alpha">The opacity of the color. (Note that the final color will be premultiplied by this value).</param>
        /// <returns>A new <see cref="Color"/> from a given hexidecimal value.</returns>
        public static Color CreateFromHex(string hex, float alpha = 1)
        {
            string formatted = hex.ToLowerInvariant().Trim();
            formatted = formatted.Substring(0, 1) == "#" ? formatted.Substring(1) : formatted;

            if (!Regex.IsMatch(formatted, "^[\\da-f]{6}$"))
                throw new RelatusException("The given string is not a valid color.", new ArgumentException());

            uint asInt = (uint)Convert.ToInt32(formatted, 16);

            return CreateFromHex(asInt, alpha);
        }

        private static int ValidateColorValue(float value)
        {
            int validated = (int)value;
            validated = Math.Max(0, validated);
            validated = Math.Min(255, validated);

            return validated;
        }

        private static float ValidateAlpha(float alpha)
        {
            float validated = alpha;
            validated = Math.Max(0, validated);
            validated = Math.Min(1, validated);

            return validated;
        }
    }
}
