using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Handles the entire life cycle of any registered <see cref="SpriteData"/>.
    /// </summary>
    public static class SpriteManager
    {
        private static readonly ResourceHandler<SpriteData> spriteDataLookup;

        static SpriteManager()
        {
            spriteDataLookup = new ResourceHandler<SpriteData>();
        }

        #region Handle Sprite Data
        /// <summary>
        /// Register new <see cref="SpriteData"/> to be managed by Relatus.
        /// </summary>
        /// <param name="name">The name the new sprite data should have.</param>
        /// <param name="x">The pixel coordinate's x-value, relative the spritesheet's top-left, that the sprite should start at.</param>
        /// <param name="y">The pixel coordinate's y-value, relative the spritesheet's top-left, that the sprite should start at.</param>
        /// <param name="width">The width of the sprite.</param>
        /// <param name="height">The height of the sprite.</param>
        /// <param name="spriteSheet">The name of the image registered via <see cref="AssetManager.LoadImage(string, string)"/> that the sprite data is referencing.</param>
        public static void RegisterSpriteData(string name, int x, int y, int width, int height, string spriteSheet)
        {
            spriteDataLookup.Register(name, new SpriteData(x, y, width, height, spriteSheet));
        }

        /// <summary>
        /// Get <see cref="SpriteData"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the sprite data that was previously registered.</param>
        /// <returns>The registered sprite data with the given name.</returns>
        public static SpriteData GetSpriteData(string name)
        {
            return spriteDataLookup.Get(name);
        }

        /// <summary>
        /// Remove registered <see cref="SpriteData"/>.
        /// </summary>
        /// <param name="name">The name given to the sprite data that was previously registered.</param>
        public static void RemoveSpriteData(string name)
        {
            spriteDataLookup.Remove(name);
        }
        #endregion
    }
}
