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

        //public Matrix Transform { get; private set; }
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
            position = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetTarget(float x, float y, float z)
        {
            target = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetUp(float x, float y, float z)
        {
            up = new Vector3(x, y, z);

            view = Matrix.CreateLookAt(position, target, up);
            WVP = world * view * projection;

            return this;
        }

        public Camera SetWorld(Matrix world)
        {
            this.world = world;

            WVP = world * view * projection;

            return this;
        }

        public Camera SetBounds(float width, float height)
        {
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

        //private void Initialize()
        //{
        //    UpdatePositions();
        //    UpdateMatrices();
        //}

        //private void UpdatePositions()
        //{
        //    Zoom = WindowManager.Scale;

        //    if (WindowManager.WideScreenSupported)
        //    {
        //        Bounds = new RectangleF
        //        (
        //            (int)TopLeft.X - WindowManager.PillarBox,
        //            (int)TopLeft.Y - WindowManager.LetterBox,
        //            WindowManager.PixelWidth + (int)Math.Ceiling(WindowManager.PillarBox * 2),
        //            WindowManager.PixelHeight + (int)Math.Ceiling(WindowManager.LetterBox * 2)
        //        );
        //        cameraCenter = new Vector3(Bounds.Width / 2 - WindowManager.PillarBox, Bounds.Height / 2 - WindowManager.LetterBox, 0);
        //    }
        //    else
        //    {
        //        Bounds = new RectangleF
        //        (
        //            (int)TopLeft.X,
        //            (int)TopLeft.Y,
        //            WindowManager.PixelWidth,
        //            WindowManager.PixelHeight
        //        );
        //        cameraCenter = new Vector3(Bounds.Width / 2, Bounds.Height / 2, 0);
        //    }

        //    position = new Vector3(Center.X, -Center.Y, 1);
        //    target = new Vector3(position.X, position.Y, 0);
        //}

        //private void UpdateMatrices()
        //{
        //    Transform =
        //        // M = R * T * S
        //        // Translate the transform matrix to the inverse of the camera's center.
        //        Matrix.CreateTranslation(-cameraCenter) *
        //        // Rotate the camera relative to the center of the camera bounds.
        //        Matrix.CreateRotationZ(Rotation) *
        //        // Translate the transform matrix to the transform matrix to the inverse of the camera's top left.
        //        Matrix.CreateTranslation(-TopLeft) *

        //        // Scale the transform matrix by the camera's zoom.
        //        Matrix.CreateScale(Zoom + zoomOffset) *

        //        // Anchor the transform matrix to the center of the screen instead of the top left.
        //        Matrix.CreateTranslation(new Vector3(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2, 0)) *
        //        Matrix.Identity;

        //    world =
        //        // To be honest, I am not sure why this works.
        //        // Essentially it rotates the World matrix exactly how the Transform matrix works.
        //        Matrix.CreateTranslation(-cameraCenter) *
        //        Matrix.CreateRotationZ(Rotation) *
        //        Matrix.CreateTranslation(cameraCenter) *

        //        // In a right-handed coordinate system, the y-axis points upwards. By default, positioning a polygon follows this logic.
        //        // However, when drawing a sprite using SpriteBatch the y-axis points downwards.
        //        // Since SpriteBatch is predominately used, the World matrix is rotated so the y-axis also points downwards.
        //        Matrix.CreateRotationX(MathHelper.Pi) *
        //        Matrix.Identity;

        //    view =
        //        Matrix.CreateLookAt(position, target, Vector3.Up);

        //    projection =
        //        Matrix.CreateOrthographic(WindowManager.WindowWidth / (Zoom + zoomOffset), WindowManager.WindowHeight / (Zoom + zoomOffset), 0, 64);

        //    WVP = world * view * projection;
        //}

        public virtual void Update()
        {

        }
    }
}
