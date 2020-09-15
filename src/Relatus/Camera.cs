using Microsoft.Xna.Framework;
using System;

namespace Relatus
{
    public enum ProjectionType
    {
        None,
        Orthographic,
        Perspective
    }

    public class Camera
    {
        public Matrix WVP { get; private set; }

        public RectangleF Bounds => new RectangleF(position.X, position.Y, width, height);

        public Vector3 Position
        {
            get => position;
            set => SetPosition(value.X, value.Y, value.Z);
        }

        public Vector3 Target
        {
            get => target;
            set => SetTarget(value.X, value.Y, value.Z);
        }

        public Vector3 Up
        {
            get => up;
            set => SetUp(value.X, value.Y, value.Z);
        }

        public Matrix World
        {
            get => world;
            set => SetWorld(value);
        }

        private Vector3 target;
        private Vector3 position;
        private Vector3 up;

        private Matrix world;
        private Matrix view;
        private Matrix projection;

        private readonly ProjectionType projectionType;
        private readonly float near;
        private readonly float far;
        private float width;
        private float height;

        internal Camera(float width, float height, float near, float far, ProjectionType projectionType)
        {
            target = Vector3.Zero;
            position = new Vector3(target.X, target.Y, target.Z + 1);
            up = Vector3.Up;

            this.width = width;
            this.height = height;
            this.near = near;
            this.far = far;
            this.projectionType = projectionType;

            world = Matrix.Identity;
            view = Matrix.CreateLookAt(position, target, up);

            switch (this.projectionType)
            {
                case ProjectionType.Orthographic:
                    projection = Matrix.CreateOrthographic(width, height, near, far);
                    break;
                case ProjectionType.Perspective:
                    projection = Matrix.CreatePerspective(width, height, near, far);
                    break;
            }

            WVP = world * view * projection;
        }

        public static Camera CreateOrthographic(float width, float height, float near, float far)
        {
            return new Camera(width, height, near, far, ProjectionType.Orthographic);
        }

        public static Camera CreateOrthographicOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            return new Camera(right - left, top - bottom, near, far, ProjectionType.Orthographic);
        }

        public static Camera CreatePerspective(float width, float height, float near, float far)
        {
            return new Camera(width, height, near, far, ProjectionType.Perspective);
        }

        public static Camera CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            return new Camera(right - left, top - bottom, near, far, ProjectionType.Perspective);
        }

        public static Camera CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float near, float far)
        {
            float width = 2 * near * (float)Math.Tan(fieldOfView * 0.5f) * aspectRatio;
            float height = width / aspectRatio;

            return new Camera(width, height, near, far, ProjectionType.Perspective);
        }

        public Camera SetPosition(Vector3 position)
        {
            if (this.position == position)
                return this;

            this.position = position;

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public Camera SetTarget(Vector3 target)
        {
            if (this.target == target)
                return this;

            this.target = target;

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetTarget(float x, float y, float z)
        {
            return SetTarget(new Vector3(x, y, z));
        }

        public Camera SetUp(Vector3 up)
        {
            if (this.up == up)
                return this;

            this.up = up;

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetUp(float x, float y, float z)
        {
            return SetUp(new Vector3(x, y, z));
        }

        public Camera SetWorld(Matrix world)
        {
            if (this.world == world)
                return this;

            this.world = world;

            WVP = world * view * projection;

            return this;
        }

        public Camera SetDimensions(float width, float height)
        {
            if (this.width == width && this.height == height)
                return this;

            this.width = width;
            this.height = height;

            switch (projectionType)
            {
                case ProjectionType.Orthographic:
                    projection = Matrix.CreateOrthographic(this.width, this.height, near, far);
                    break;
                case ProjectionType.Perspective:
                    projection = Matrix.CreatePerspective(this.width, this.height, near, far);
                    break;
            }

            WVP = world * view * projection;

            return this;
        }
    }
}
