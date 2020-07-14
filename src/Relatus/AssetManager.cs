using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Handles the entire life cycle of all of the following registered assets: <see cref="Texture2D"/>, <see cref="Effect"/>, <see cref="SoundEffect"/>, and <see cref="BMFont"/>.
    /// </summary>
    public static class AssetManager
    {
        private static readonly ResourceHandler<Texture2D> textures;
        private static readonly ResourceHandler<Effect> effects;
        private static readonly ResourceHandler<SoundEffect> soundEffects;
        private static readonly ResourceHandler<BMFont> fonts;

        static AssetManager()
        {
            textures = new ResourceHandler<Texture2D>();
            effects = new ResourceHandler<Effect>();
            soundEffects = new ResourceHandler<SoundEffect>();
            fonts = new ResourceHandler<BMFont>();
        }

        #region Handle Images
        /// <summary>
        /// Load a <see cref="Texture2D"/> into memory.
        /// </summary>
        /// <param name="name">The name that the image being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the image contained in the Content folder.</param>
        public static void LoadImage(string name, string path)
        {
            textures.Register(name, Engine.Instance.Content.Load<Texture2D>(path));
        }

        /// <summary>
        /// Get a <see cref="Texture2D"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded image.</param>
        /// <returns>The loaded <see cref="Texture2D"/> associated with the given name.</returns>
        public static Texture2D GetImage(string name)
        {
            return textures.Get(name);
        }

        /// <summary>
        /// Unload an already loaded <see cref="Texture2D"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded image.</param>
        public static void UnloadImage(string name)
        {
            textures.Remove(name);
        }
        #endregion

        #region Handle Effects
        /// <summary>
        /// Load a <see cref="Effect"/> into memory.
        /// </summary>
        /// <param name="name">The name that the effect being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the effect contained in the Content folder.</param>
        public static void LoadEffect(string name, string path)
        {
            effects.Register(name, Engine.Instance.Content.Load<Effect>(path));
        }

        /// <summary>
        /// Get a <see cref="Effect"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded effect.</param>
        /// <returns>The loaded <see cref="Effect"/> associated with the given name.</returns>
        public static Effect GetEffect(string name)
        {
            return effects.Get(name);
        }

        /// <summary>
        /// Unload an already loaded <see cref="Effect"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded effect.</param>
        public static void UnloadEffect(string name)
        {
            effects.Remove(name);
        }
        #endregion

        #region Handle SoundEffects
        /// <summary>
        /// Load a <see cref="SoundEffect"/> into memory.
        /// </summary>
        /// <param name="name">The name that the .wav file being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the .wav file contained in the Content folder.</param>
        public static void LoadSoundEffect(string name, string path)
        {
            soundEffects.Register(name, Engine.Instance.Content.Load<SoundEffect>(path));
        }

        /// <summary>
        /// Get a <see cref="SoundEffect"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded sound effect.</param>
        /// <returns>The loaded <see cref="SoundEffect"/> associated with the given name.</returns>
        public static SoundEffect GetSoundEffect(string name)
        {
            return soundEffects.Get(name);
        }

        /// <summary>
        /// Unload an already loaded <see cref="SoundEffect"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded sound effect.</param>
        public static void UnloadSoundEffect(string name)
        {
            soundEffects.Remove(name);
        }
        #endregion

        #region Handle BMFonts
        /// <summary>
        /// Load a <see cref="BMFont"/> into memory.
        /// </summary>
        /// <param name="name">The name that the BMFont being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the BMFont contained in the Content folder.</param>
        public static void LoadFont(string name, string path)
        {
            fonts.Register(name, Engine.Instance.Content.Load<BMFont>(path));
        }

        /// <summary>
        /// Get a <see cref="BMFont"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded font.</param>
        /// <returns>The loaded <see cref="BMFont"/> associated with the given name.</returns>
        public static BMFont GetFont(string name)
        {
            return fonts.Get(name);
        }

        /// <summary>
        /// Get a <see cref="BMFont"/> that was already loaded into memory.
        /// </summary>
        /// <param name="fontType">The basic font type you want to get.</param>
        /// <returns>The loaded <see cref="BMFont"/> associated with the given name.</returns>
        public static BMFont GetFont(FontType fontType)
        {
            return fonts.Get($"Relatus_{fontType}");
        }

        /// <summary>
        /// Unload an already loaded <see cref="BMFont"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded font.</param>
        public static void UnloadFont(string name)
        {
            fonts.Remove(name);
        }
        #endregion

        internal static void LoadContent()
        {
            LoadEffect("Relatus_BMFontShader", "Assets/Effects/BMFontShader");
            LoadEffect("Relatus_PolygonShader", "Assets/Effects/Polygon");

            LoadFont("Relatus_Probity", "Assets/Fonts/probity");
            LoadFont("Relatus_Sparge", "Assets/Fonts/sparge");
        }

        internal static void UnloadContent()
        {
            textures.Dispose();
            effects.Dispose();
            soundEffects.Dispose();
            fonts.Dispose();
        }
    }
}
