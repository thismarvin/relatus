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
        TopLeftAlign,
        /// <summary>
        /// The RightAlign camera will also never move, but if WideScreenSupported is true, anything drawn will become top-right aligned to use the extra window space.
        /// </summary>
        TopRightAlign,
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

            RegisterCamera(new Camera($"Relatus_{CameraType.Static}"));
            RegisterCamera(new Camera($"Relatus_{CameraType.TopLeftAlign}"));
            RegisterCamera(new Camera($"Relatus_{CameraType.TopRightAlign}"));

            WindowManager.WindowChanged += HandleWindowChange;
        }

        #region Handle Cameras
        /// <summary>
        /// Register a <see cref="Camera"/> to be managed by Relatus.
        /// </summary>
        /// <param name="camera">The camera you want to be registered.</param>
        public static void RegisterCamera(Camera camera)
        {
            cameras.Register(camera.Name, camera);
        }

        /// <summary>
        /// Get a <see cref="Camera"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the camera that was previously registered.</param>
        /// <returns>The registered camera with the given name.</returns>
        public static Camera GetCamera(string name)
        {
            return cameras.Get(name);
        }

        /// <summary>
        /// Get a <see cref="Camera"/> that was registered by Relatus.
        /// </summary>
        /// <param name="cameraType">The basic camera you want to get.</param>
        /// <returns>The registered camera with the given name.</returns>
        public static Camera GetCamera(CameraType cameraType)
        {
            return GetCamera($"Relatus_{cameraType}");
        }

        /// <summary>
        /// Remove a registered <see cref="Camera"/>.
        /// </summary>
        /// <param name="name">The name given to the camera that was previously registered.</param>
        public static void RemoveCamera(string name)
        {
            cameras.Remove(name);
        }
        #endregion

        private static void HandleWindowChange(object sender, EventArgs e)
        {
            ResetCameras();
        }

        private static void ResetCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.Reset();
            }
        }

        private static void ManageManagedCameras()
        {
            if (WindowManager.WideScreenSupported)
            {
                GetCamera(CameraType.TopLeftAlign).SetTopLeft(WindowManager.PillarBox, WindowManager.LetterBox);
                GetCamera(CameraType.TopRightAlign).SetTopLeft(-WindowManager.PillarBox, -WindowManager.LetterBox);
            }
            else
            {
                GetCamera(CameraType.TopLeftAlign).SetTopLeft(0, 0);
                GetCamera(CameraType.TopRightAlign).SetTopLeft(0, 0);
            }

            GetCamera(CameraType.Static).SetTopLeft(0, 0);
        }

        private static void UpdateCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.Update();
            }
        }

        internal static void Update()
        {
            ManageManagedCameras();
            UpdateCameras();
        }
    }
}
