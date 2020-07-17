using Microsoft.Xna.Framework;
using Relatus.Maths;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

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
        public string Name { get; private set; }
        public Matrix WVP { get; private set; }
        public Matrix SpriteTransform { get; private set; }

        public RectangleF Bounds => new RectangleF(position.X, position.Y, width, height);

        public Vector3 Position
        {
            get => position;
            set
            {
                SetPosition(value.X, value.Y, value.Z);
            }
        }

        public Vector3 Target
        {
            get => target;
            set
            {
                SetTarget(value.X, value.Y, value.Z);
            }
        }

        public Vector3 Up
        {
            get => up;
            set
            {
                SetUp(value.X, value.Y, value.Z);
            }
        }

        public Matrix World
        {
            get => world;
            set
            {
                SetWorld(value);
            }
        }

        //public float Zoom { get; private set; }
        //public float Rotation { get; private set; }

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

        internal Camera(string name, float width, float height, float near, float far, ProjectionType projectionType)
        {
            Name = name;
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
            SpriteTransform = CreateTransform();

            WindowManager.WindowResize += HandleWindowResize;
        }

        internal void HandleWindowResize(object sender, EventArgs e)
        {
            SpriteTransform = CreateTransform();
        }

        public static Camera CreateOrthographic(string name, float width, float height, float near, float far)
        {
            return new Camera(name, width, height, near, far, ProjectionType.Orthographic);
        }

        public static Camera CreateOrthographicOffCenter(string name, float left, float right, float bottom, float top, float near, float far)
        {
            return new Camera(name, right - left, top - bottom, near, far, ProjectionType.Orthographic);
        }

        public static Camera CreatePerspective(string name, float width, float height, float near, float far)
        {
            return new Camera(name, width, height, near, far, ProjectionType.Perspective);
        }

        public static Camera CreatePerspectiveOffCenter(string name, float left, float right, float bottom, float top, float near, float far)
        {
            return new Camera(name, right - left, top - bottom, near, far, ProjectionType.Perspective);
        }

        public static Camera CreatePerspectiveFieldOfView(string name, float fieldOfView, float aspectRatio, float near, float far)
        {
            float width = 2 * (float)Math.Tan(fieldOfView / 2);
            float height = width / aspectRatio;

            return new Camera(name, width, height, near, far, ProjectionType.Perspective);
        }

        public Camera SetPosition(float x, float y, float z)
        {
            if (position.X == x && position.Y == y && position.Z == z)
                return this;

            position = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;
            SpriteTransform = CreateTransform();

            return this;
        }

        public Camera SetTarget(float x, float y, float z)
        {
            if (target.X == x && target.Y == y && target.Z == z)
                return this;

            target = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetUp(float x, float y, float z)
        {
            if (up.X == x && up.Y == y && up.Z == z)
                return this;

            up = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetWorld(Matrix world)
        {
            if (this.world == world)
                return this;

            this.world = world;

            WVP = world * view * projection;

            return this;
        }

        public Camera SetBounds(float width, float height)
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

        private Matrix CreateTransform()
        {
            return
                Matrix.CreateTranslation(WindowManager.PillarBox - position.X, WindowManager.LetterBox - position.Y, 0) *
                Matrix.CreateScale(WindowManager.Scale);
        }
    }
}
