using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public float Top => Y;
        public float Bottom => Y - Height;
        public float Left => X;
        public float Right => X + Width;

        public Vector2 Position => new Vector2(X, Y);
        public Vector2 Center => new Vector2(X + Width * 0.5f, Y - Height * 0.5f);

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }

        public bool Intersects(RectangleF rectangle)
        {
            return (Left < rectangle.Right && Right > rectangle.Left && Bottom < rectangle.Top && Top > rectangle.Bottom);
        }

        public bool Contains(float x, float y)
        {
            return (Left <= x && Right >= x && Bottom <= y && Top >= y);
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(RectangleF rectangle)
        {
            return (Left <= rectangle.Left && Right >= rectangle.Right && Bottom <= rectangle.Bottom && Top >= rectangle.Top);
        }

        public Vector2 GetResolution(RectangleF rectangle)
        {
            if (!Intersects(rectangle))
                return Vector2.Zero;

            float xMin = Width * 0.5f + rectangle.Width * 0.5f;
            float yMin = Height * 0.5f + rectangle.Height * 0.5f;

            float xDelta = Math.Abs(Center.X - rectangle.Center.X);
            float yDelta = Math.Abs(Center.Y - rectangle.Center.Y);

            if (MathExt.RemapRange(xDelta, 0, xMin, 0, 1) > MathExt.RemapRange(yDelta, 0, yMin, 0, 1))
            {
                float xResolution = (xMin - xDelta) * (X < rectangle.X ? -1 : 1);

                // Left
                if (xResolution < 0)
                    return new Vector2(-(Right - rectangle.Left), 0);
                // Right
                if (xResolution > 0)
                    return new Vector2(rectangle.Right - Left, 0);
            }
            else
            {
                float yResolution = (yMin - yDelta) * (Y < rectangle.Y ? -1 : 1);

                // Bottom
                if (yResolution < 0)
                    return new Vector2(0, -(Top - rectangle.Bottom));
                // Top
                if (yResolution > 0)
                    return new Vector2(0, rectangle.Top - Bottom);
            }

            return Vector2.Zero;
        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height);
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !(a == b);
        }

        public bool Equals(RectangleF other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj == null | !(obj is RectangleF))
                return false;

            return (RectangleF)obj == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public override string ToString()
        {
            return base.ToString() + " " + $": Position:(X:{X}, Y:{Y}), Dimensions:(W:{Width}, H:{Height})";
        }
    }
}
