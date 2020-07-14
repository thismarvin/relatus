using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Relatus.Debug;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Responsible for maintaining and displaying <see cref="DebugEntry"/>'s.
    /// (On PC, in-game debugging can be toggled by pressing F3).
    /// </summary>
    public static class DebugManager
    {
        public static bool Debugging { get; set; }
        public static bool ShowWireFrame { get; set; }

        private static readonly ResourceHandler<DebugEntry> debugEntries;

        private static readonly DebugEntry fps;
        private static readonly DebugEntry scene;
        private static readonly DebugEntry entity;

        static DebugManager()
        {
            debugEntries = new ResourceHandler<DebugEntry>();

            fps = new DebugEntry("Relatus_FPS", "{0} FPS");
            RegisterDebugEntry(fps);

            scene = new DebugEntry("Relatus_Scene", "SCENE: {0}");
            RegisterDebugEntry(scene);

            entity = new DebugEntry("Relatus_Entities", "E: {0}");
            RegisterDebugEntry(entity);
        }

        #region Handle DebugEntries
        /// <summary>
        /// Register a <see cref="DebugEntry"/> to be managed by Relatus.
        /// </summary>
        /// <param name="debugEntry">The debug entry you want to register.</param>
        public static void RegisterDebugEntry(DebugEntry debugEntry)
        {
            debugEntries.Register(debugEntry.Name, debugEntry);
        }

        /// <summary>
        /// Get a <see cref="DebugEntry"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the debug entry that was previously registered.</param>
        /// <returns>The registered debug entry with the given name.</returns>
        public static DebugEntry GetDebugEntry(string name)
        {
            return debugEntries.Get(name);
        }

        /// <summary>
        /// Remove a registered <see cref="DebugEntry"/>.
        /// </summary>
        /// <param name="name">The name given to the debug entry that was previously registered.</param>
        public static void RemoveDebugEntry(string name)
        {
            debugEntries.Remove(name);
        }
        #endregion

        // TODO: This is soooo janky!
        internal static Vector2 NextDebugEntryPosition()
        {
            int padding = 4;
            int textHeight = 8;
            int lineHeight = textHeight + 2;

            return new Vector2(padding, padding + debugEntries.Count * lineHeight);
        }

        private static void UpdateInput()
        {
            if (Input.KeyboardExt.Pressed(Keys.F3))
            {
                Debugging = !Debugging;
            }

            if (!Debugging)
                return;

            if (Input.KeyboardExt.Pressing(Keys.LeftShift) && Input.KeyboardExt.Pressed(Keys.D1))
            {
                ShowWireFrame = !ShowWireFrame;
            }
        }

        private static void UpdateInfo()
        {
            fps.SetInformation(Math.Round(WindowManager.FPS));
            scene.SetInformation(SceneManager.CurrentScene.Name);
            entity.SetInformation(SceneManager.CurrentScene.EntityCount);
        }

        private static void DrawDebugEntries()
        {
            if (!Debugging)
                return;

            foreach (DebugEntry debugEntry in debugEntries)
            {
                debugEntry.Draw(CameraManager.GetCamera(CameraType.TopLeftAlign));
            }
        }

        internal static void Update()
        {
            UpdateInput();
            UpdateInfo();
        }

        internal static void Draw()
        {
            DrawDebugEntries();
        }
    }
}
