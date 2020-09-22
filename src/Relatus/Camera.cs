using Microsoft.Xna.Framework;

namespace Relatus
{
    public abstract class Camera
    {
        public Transform Transform
        {
            get => transform;
            set => SetTransform(value);
        }

        public Vector3 Position
        {
            get => position;
            set => SetPosition(value);
        }
        public Vector3 Forward
        {
            get => forward;
            set => SetForward(value);
        }
        public Vector3 Up
        {
            get => up;
            set => SetUp(value);
        }

        public Matrix Projection
        {
            get => projection;
            protected set => SetProjection(value);
        }

        public Matrix View => CalculateView();
        public Matrix WVP => CalculateWVP();

        protected Matrix view;
        protected Matrix projection;
        protected float near;
        protected float far;

        protected Transform transform;

        protected Vector3 position;
        protected Vector3 forward;
        protected Vector3 up;

        private Matrix wvpCache;
        private bool viewModified;
        private bool wvpModified;

        protected Camera()
        {
            Transform = new Transform();

            forward = Vector3.Forward;
            up = Vector3.Up;

            view = Matrix.Identity;
            projection = Matrix.Identity;

            viewModified = true;
            wvpModified = true;
            wvpCache = Matrix.Identity;
        }

        public Camera SetTransform(Transform transform)
        {
            this.transform = transform;

            return this;
        }

        public Camera SetPosition(Vector3 position)
        {
            if (this.position == position)
                return this;

            this.position = position;

            viewModified = true;
            wvpModified = true;

            return this;
        }

        public Camera SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public Camera SetForward(Vector3 forward)
        {
            if (this.forward == forward)
                return this;

            this.forward = forward;

            viewModified = true;
            wvpModified = true;

            return this;

        }

        public Camera SetForward(float x, float y, float z)
        {
            return SetForward(x, y, z);
        }

        public Camera SetUp(Vector3 up)
        {
            if (this.up == up)
                return this;

            this.up = up;

            viewModified = true;
            wvpModified = true;

            return this;
        }

        public Camera SetUp(float x, float y, float z)
        {
            return SetUp(new Vector3(x, y, z));
        }

        protected Camera SetProjection(Matrix projection)
        {
            if (this.projection == projection)
                return this;

            this.projection = projection;

            wvpModified = true;

            return this;
        }

        private Matrix CalculateView()
        {
            if (viewModified)
            {
                view = Matrix.CreateLookAt(position, position + forward, up);
                viewModified = false;
            }

            return view;
        }

        private Matrix CalculateWVP()
        {
            if (wvpModified)
            {
                CalculateView();

                wvpCache = Transform.Matrix * view * projection;
                wvpModified = false;
            }

            return wvpCache;
        }
    }
}
