using Microsoft.Xna.Framework;

namespace Relatus
{
    public class PerspectiveCamera : Camera
    {
        public float FOV { get; set; }
        public float AspectRatio { get; set; }

        public PerspectiveCamera(float fov, float aspectRatio, float near, float far) : base()
        {
            FOV = fov;
            AspectRatio = aspectRatio;
            this.near = near;
            this.far = far;

            projection = Matrix.CreatePerspectiveFieldOfView(FOV, AspectRatio, near, far);
        }
    }
}
