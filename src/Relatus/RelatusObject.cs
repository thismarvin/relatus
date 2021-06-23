using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Relatus
{
    public abstract class RelatusObject : IComparable<RelatusObject>
    {
        public float X
        {
            get => x;
            set => SetPosition(value, y, z);
        }

        public float Y
        {
            get => y;
            set => SetPosition(x, value, z);
        }

        public float Z
        {
            get => z;
            set => SetPosition(x, y, value);
        }

        public float Width
        {
            get => width;
            set => SetDimensions(value, height, depth);
        }

        public float Height
        {
            get => height;
            set => SetDimensions(width, value, depth);
        }

        public float Depth
        {
            get => depth;
            set => SetDimensions(width, height, value);
        }

        public Vector3 Position
        {
            get => new Vector3(x, y, z);
            set => SetPosition(value.X, value.Y, value.Z);
        }

        public Vector3 Center
        {
            get => new Vector3(x + width * 0.5f, y - height * 0.5f, z - depth * 0.5f);
            set => SetCenter(value.X, value.Y, value.Z);
        }

        private float x;
        private float y;
        private float z;
        private float width;
        private float height;
        private float depth;

        public RelatusObject(float x, float y, float z, float width, float height, float depth)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public RelatusObject() : this(0, 0, 0, 0, 0, 0)
        {
        }

        public virtual RelatusObject SetPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            return this;
        }

        public virtual RelatusObject SetCenter(float x, float y, float z)
        {
            this.x = x - width * 0.5f;
            this.y = y + height * 0.5f;
            this.z = z + depth * 0.5f;

            return this;
        }

        public virtual RelatusObject SetDimensions(float width, float height, float depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;

            return this;
        }

        public int CompareTo(RelatusObject other)
        {
            return z.CompareTo(other.Z);
        }

        public override string ToString()
        {
            return $"<{x}, {y}, {z}>, [{width} x {height} x {depth}]";
        }
    }
}
