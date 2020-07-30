using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public static class Batcher
    {
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
