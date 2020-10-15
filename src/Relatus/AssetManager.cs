using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using Relatus.Utilities;

namespace Relatus
{
    /// <summary>
    /// Handles the entire life cycle of all of the following registered assets: <see cref="Texture2D"/>, <see cref="Effect"/>, <see cref="SoundEffect"/>, and <see cref="BMFont"/>.
    /// </summary>
    public static class AssetManager
    {
        private static readonly ContentManager contentManager;

        private static readonly ResourceHandler<Texture2D> textures;
        private static readonly ResourceHandler<Effect> effects;
        private static readonly ResourceHandler<SoundEffect> soundEffects;
        private static readonly ResourceHandler<BMFont> fonts;
        private static readonly ResourceHandler<SpriteAtlas> atlases;
        private static readonly ResourceHandler<Model> models;

        static AssetManager()
        {
            contentManager = Engine.Instance.Content;

            textures = new ResourceHandler<Texture2D>();
            effects = new ResourceHandler<Effect>();
            soundEffects = new ResourceHandler<SoundEffect>();
            fonts = new ResourceHandler<BMFont>();
            atlases = new ResourceHandler<SpriteAtlas>();
            models = new ResourceHandler<Model>();
        }

        #region Handle Images
        /// <summary>
        /// Load a <see cref="Texture2D"/> into memory.
        /// </summary>
        /// <param name="name">The name that the image being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the image contained in the Content folder.</param>
        public static void LoadImage(string name, string path)
        {
            textures.Register(name, contentManager.Load<Texture2D>(path));
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
            effects.Register(name, contentManager.Load<Effect>(path));
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
            soundEffects.Register(name, contentManager.Load<SoundEffect>(path));
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
            fonts.Register(name, contentManager.Load<BMFont>(path).LoadPages(path));
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
        /// Unload an already loaded <see cref="BMFont"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded font.</param>
        public static void UnloadFont(string name)
        {
            fonts.Remove(name);
        }
        #endregion

        #region Handle SpriteAtlases
        /// <summary>
        /// Load a <see cref="SpriteAtlas"/> into memory.
        /// </summary>
        /// <param name="name">The name that the sprite atlas being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the sprite atlas contained in the Content folder.</param>
        public static void LoadSpriteAtlas(string name, string path)
        {
            atlases.Register(name, contentManager.Load<SpriteAtlas>(path).LoadSpriteSheet(path));
        }

        /// <summary>
        /// Get a <see cref="SpriteAtlas"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded sprite atlas.</param>
        /// <returns>The loaded <see cref="SpriteAtlas"/> associated with the given name.</returns>
        public static SpriteAtlas GetSpriteAtlas(string name)
        {
            return atlases.Get(name);
        }

        /// <summary>
        /// Unload an already loaded <see cref="SpriteAtlas"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded sprite atlas.</param>
        public static void UnloadSpriteAtlas(string name)
        {
            atlases.Remove(name);
        }
        #endregion

        #region Handle Models 
        /// <summary>
        /// Load a <see cref="Model"/> into memory.
        /// </summary>
        /// <param name="name">The name that the model being loaded will be referenced as.</param>
        /// <param name="path">The relative path to the model contained in the Content folder.</param>
        public static void LoadModel(string name, string path)
        {
            models.Register(name, contentManager.Load<Model>(path));
        }

        /// <summary>
        /// Get a <see cref="Model"/> that was already loaded into memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded model.</param>
        /// <returns>The loaded <see cref="Model"/> associated with the given name.</returns>
        public static Model GetModel(string name)
        {
            return models.Get(name);
        }

        /// <summary>
        /// Unload an already loaded <see cref="Model"/> from memory.
        /// </summary>
        /// <param name="name">The name assigned to a previously loaded model.</param>
        public static void UnloadModel(string name)
        {
            models.Remove(name);
        }
        #endregion

        internal static void LoadContent()
        {
            LoadEffect("Relatus_RelatusEffect", "Relatus.Content/RelatusEffect");
            LoadEffect("Relatus_3DEffect", "Relatus.Content/Relatus3DEffect");
            LoadEffect("Relatus_BMFontShader", "Relatus.Content/BMFontShader");

            LoadFont("Relatus_Probity", "Relatus.Content/probity");
        }

        internal static void UnloadContent()
        {
            textures.Dispose();
            effects.Dispose();
            soundEffects.Dispose();
            fonts.Dispose();
            atlases.Dispose();
            models.Dispose();
        }
    }
}
