using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    static class Batcher
    {
        /// <summary>
        /// Creates a temporary <see cref="SpriteCollection"/> that batches and draws a given array of sprites.
        /// </summary>
        /// <param name="sprites">The array of sprites that will be batched and drawn together.</param>
        /// <param name="camera">The camera used to draw the sprites.</param>
        public static void DrawSprites(Sprite[] sprites, Camera camera)
        {
            SpriteCollection spriteCollection = new SpriteCollection();
            spriteCollection
                .SetCollection(sprites)
                .Draw(camera);
        }

        /// <summary>
        /// Creates a temporary <see cref="PolygonCollection"/> that batches and draws a given array of polygons.
        /// </summary>
        /// <param name="polygons">The array of polygons that will be batched and drawn together.</param>
        /// <param name="camera">The camera used to draw the sprites.</param>
        public static void DrawPolygons(Polygon[] polygons, Camera camera)
        {
            using (PolygonCollection polygonCollection = new PolygonCollection())
            {
                polygonCollection
                    .SetCollection(polygons)
                    .Draw(camera);
            }
        }
    }
}
