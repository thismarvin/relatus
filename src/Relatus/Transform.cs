using Microsoft.Xna.Framework;

namespace Relatus
{
    public class Transform
    {
        public Vector3 Scale
        {
            get => scale;
            set => SetScale(value);
        }
        public Vector3 Origin
        {
            get => origin;
            set => SetOrigin(value);
        }
        public Quaternion Rotation
        {
            get => rotation;
            set => SetRotation(value);
        }
        public Vector3 Translation
        {
            get => translation;
            set => SetTranslation(value);
        }

        public EulerAngles EulerAngles => CalculateEulerAngles();
        public Matrix Matrix => CalculateMatrix();
        public Matrix Normal => CalculateNormal();

        private Vector3 scale;
        private Vector3 origin;
        private Quaternion rotation;
        private Vector3 translation;

        private EulerAngles eulerAngles;
        private bool rotationModified;

        private Matrix matrix;
        private Matrix normal;
        private bool matrixModified;
        private bool normalModified;

        public Transform(Vector3 scale, Vector3 origin, Quaternion rotation, Vector3 translation)
        {
            this.scale = scale;
            this.origin = origin;
            this.rotation = rotation;
            this.translation = translation;

            rotationModified = true;
            matrixModified = true;
            normalModified = true;
        }

        public Transform() : this(Vector3.One, Vector3.Zero, Quaternion.Identity, Vector3.Zero)
        {

        }

        public Transform SetScale(Vector3 scale)
        {
            this.scale = scale;

            matrixModified = true;

            return this;
        }

        public Transform SetScale(float x, float y, float z)
        {
            return SetScale(new Vector3(x, y, z));
        }

        public Transform SetOrigin(Vector3 origin)
        {
            this.origin = origin;

            matrixModified = true;

            return this;
        }

        public Transform SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public Transform SetRotation(Quaternion rotation)
        {
            this.rotation = rotation;

            rotationModified = true;
            matrixModified = true;

            return this;
        }

        public Transform SetTranslation(Vector3 translation)
        {
            this.translation = translation;

            matrixModified = true;

            return this;
        }

        public Transform SetTranslation(float x, float y, float z)
        {
            return SetTranslation(new Vector3(x, y, z));
        }

        private EulerAngles CalculateEulerAngles()
        {
            if (rotationModified)
            {
                eulerAngles = EulerAngles.CreateFromQuaternion(rotation);
                rotationModified = false;
            }

            return eulerAngles;
        }

        private Matrix CalculateMatrix()
        {
            if (matrixModified)
            {
                matrix =
                    Matrix.CreateScale(scale) *
                    Matrix.CreateTranslation(-origin) *
                    Matrix.CreateFromQuaternion(rotation) *
                    Matrix.CreateTranslation(origin + translation);

                normalModified = true;
            }

            return matrix;
        }

        private Matrix CalculateNormal()
        {
            Matrix matrix = CalculateMatrix();

            if (normalModified)
            {
                normal = Matrix.Transpose(matrix);
                normalModified = false;
            }

            return normal;
        }
    }
}
