using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Upholds the accuracy of a <see cref="Camera"/>, and maintains a list of basic cameras.
    /// </summary>
    public static class CameraManager
    {
        /// <summary>
        /// This camera will never move, and should be used for drawing anything that must always be visible on the screen (e.g. menus, transitions, etc.).
        /// </summary>
        public static Camera Static { get; private set; }
        /// <summary>
        /// This camera will only move if <see cref="WindowManager.WideScreenSupported"/> is true. In such cases the camera will become top-left aligned to use the extra window space.
        /// </summary>
        public static Camera TopLeftAligned { get; private set; }
        /// <summary>
        /// This camera will only move if <see cref="WindowManager.WideScreenSupported"/> is true. In such cases the camera will become top-right aligned to use the extra window space.
        /// </summary>
        public static Camera TopRightAligned { get; private set; }

        static CameraManager()
        {
            Static = Camera.CreateOrthographic(WindowManager.ScaledWidth, WindowManager.ScaledHeight, 0.25f, 16);
            TopLeftAligned = Camera.CreateOrthographic(WindowManager.ScaledWidth, WindowManager.ScaledHeight, 0.25f, 16);
            TopRightAligned = Camera.CreateOrthographic(WindowManager.ScaledWidth, WindowManager.ScaledHeight, 0.25f, 16);

            WindowManager.WindowResize += HandleWindowResize;
        }

        private static void HandleWindowResize(object sender, EventArgs e)
        {
            Static.SetBounds(WindowManager.ScaledWidth, WindowManager.ScaledHeight);
            TopLeftAligned.SetBounds(WindowManager.ScaledWidth, WindowManager.ScaledHeight);
            TopRightAligned.SetBounds(WindowManager.ScaledWidth, WindowManager.ScaledHeight);
        }

        private static void ManageManagedCameras()
        {
            if (WindowManager.WideScreenSupported)
            {
                TopLeftAligned
                    .SetPosition(WindowManager.PillarBox, WindowManager.LetterBox, 1)
                    .SetTarget(WindowManager.PillarBox, WindowManager.LetterBox, 0);

                TopRightAligned
                    .SetPosition(-WindowManager.PillarBox, -WindowManager.LetterBox, 1)
                    .SetPosition(-WindowManager.PillarBox, -WindowManager.LetterBox, 0);
            }
            else
            {
                TopLeftAligned
                    .SetPosition(0, 0, 1)
                    .SetTarget(0, 0, 0);

                TopRightAligned
                    .SetPosition(0, 0, 1)
                    .SetTarget(0, 0, 0);
            }

            Static
                .SetPosition(0, 0, 1)
                .SetTarget(0, 0, 0);
        }

        internal static void Update()
        {
            ManageManagedCameras();
        }
    }
}
