using Microsoft.Xna.Framework;

namespace Relatus
{
    public class PerspectiveCamera : Camera
    {
        public float FOV { get; private set; }
        public float AspectRatio { get; private set; }

        public PerspectiveCamera() : base()
        {
        }

        public PerspectiveCamera SetProjection(float fov, float aspectRatio, float near, float far)
        {
            FOV = fov;
            AspectRatio = aspectRatio;
            this.near = near;
            this.far = far;

            Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, this.near, this.far);

            return this;
        }
    }
}
