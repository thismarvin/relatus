using Microsoft.Xna.Framework;

namespace Relatus
{
    public abstract class Camera
    {
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

        public Matrix WVP => CalculateWVP();

        protected Matrix projection;
        protected float near;
        protected float far;

        private Vector3 position;
        private Vector3 target;
        private Vector3 up;

        private Matrix world;
        private Matrix view;

        private bool wvpModified;
        private Matrix wvpCache;

        protected Camera()
        {
            world = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.Identity;

            wvpCache = Matrix.Identity;
        }

        public Camera SetPosition(Vector3 position)
        {
            if (this.position == position)
                return this;

            this.position = position;

            view = Matrix.CreateLookAt(position, target, up);
            wvpModified = true;

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
            wvpModified = true;

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
            wvpModified = true;

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
            wvpModified = true;

            return this;
        }

        private Matrix CalculateWVP()
        {
            if (wvpModified)
            {
                wvpCache = world * view * projection;
                wvpModified = false;
            }

            return wvpCache;
        }
    }
}
