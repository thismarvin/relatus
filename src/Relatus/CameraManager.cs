using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Basic camera types that are registered by default.
    /// </summary>
    public enum CameraType
    {
        /// <summary>
        /// The static camera will never move, and is used for drawing anything that must always be visible on the screen (e.g. menus, transitions, etc.).
        /// </summary>
        Static,
        /// <summary>
        /// The TopLeftAlign camera will also never move, but if WideScreenSupported is true, anything drawn will become top-left aligned to use the extra window space.
        /// </summary>
        TopLeftAligned,
        /// <summary>
        /// The RightAlign camera will also never move, but if WideScreenSupported is true, anything drawn will become top-right aligned to use the extra window space.
        /// </summary>
        TopRightAligned,
    }

    /// <summary>
    /// Upholds the accuracy of a <see cref="Camera"/>, and maintains a list of basic cameras.
    /// </summary>
    public static class CameraManager
    {
        private static readonly ResourceHandler<Camera> cameras;

        static CameraManager()
        {
            cameras = new ResourceHandler<Camera>();

            Register(Camera.CreateOrthographic($"Relatus_{CameraType.Static}", WindowManager.PixelWidth, WindowManager.PixelHeight, 1, 17));
            Register(Camera.CreateOrthographic($"Relatus_{CameraType.TopLeftAligned}", WindowManager.PixelWidth, WindowManager.PixelHeight, 1, 17));
            Register(Camera.CreateOrthographic($"Relatus_{CameraType.TopRightAligned}", WindowManager.PixelWidth, WindowManager.PixelHeight, 1, 17));
        }

        #region Handle Cameras
        /// <summary>
        /// Register a <see cref="Camera"/> to be managed by Relatus.
        /// </summary>
        /// <param name="camera">The camera you want to be registered.</param>
        public static void Register(Camera camera)
        {
            cameras.Register(camera.Name, camera);
        }

        /// <summary>
        /// Get a <see cref="Camera"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the camera that was previously registered.</param>
        /// <returns>The registered camera with the given name.</returns>
        public static Camera Get(string name)
        {
            return cameras.Get(name);
        }

        /// <summary>
        /// Get a <see cref="Camera"/> that was registered by Relatus.
        /// </summary>
        /// <param name="cameraType">The basic camera you want to get.</param>
        /// <returns>The registered camera with the given name.</returns>
        public static Camera Get(CameraType cameraType)
        {
            return Get($"Relatus_{cameraType}");
        }

        /// <summary>
        /// Remove a registered <see cref="Camera"/>.
        /// </summary>
        /// <param name="name">The name given to the camera that was previously registered.</param>
        public static void Remove(string name)
        {
            cameras.Remove(name);
        }
        #endregion

        private static void ManageManagedCameras()
        {
            if (WindowManager.WideScreenSupported)
            {
                Get(CameraType.TopLeftAligned).SetPosition(WindowManager.PillarBox, WindowManager.LetterBox, 1);
                Get(CameraType.TopRightAligned).SetPosition(-WindowManager.PillarBox, -WindowManager.LetterBox, 1);
            }
            else
            {
                Get(CameraType.TopLeftAligned).SetPosition(0, 0, 1);
                Get(CameraType.TopRightAligned).SetPosition(0, 0, 1);
            }

            Get(CameraType.Static).SetPosition(0, 0, 1);
        }

        internal static void Update()
        {
            ManageManagedCameras();
        }
    }
}
