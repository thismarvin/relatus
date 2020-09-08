using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class GeometryBatcher : Batcher<Polygon>
    {
        private readonly List<List<Polygon>> polygons;
        private int index;

        private static readonly GraphicsDevice graphicsDevice;

        static GeometryBatcher()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        internal GeometryBatcher()
        {
            polygons = new List<List<Polygon>>();
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;
        }

        public override Batcher<Polygon> Begin()
        {
            polygons.Clear();

            polygons.Add(new List<Polygon>((int)batchSize));
            index = 0;

            return this;
        }

        public override Batcher<Polygon> Add(Polygon polygon)
        {
            polygons[^1].Add(polygon);
            index++;

            if (index >= batchSize)
            {
                polygons.Add(new List<Polygon>((int)batchSize));
                index = 0;
            }

            return this;
        }

        public override Batcher<Polygon> End()
        {
            if (camera == null)
                throw new RelatusException("A Camera has not been attached yet. Make sure to call AttachCamera(camera).", new ArgumentNullException());

            for (int i = 0; i < polygons.Count; i++)
            {
                if (i + 1 == polygons.Count)
                {
                    using PolygonCollection polygonCollection = new PolygonCollection(execution, (uint)index, polygons[i]);
                    polygonCollection.Draw(camera);
                }
                else
                {
                    using PolygonCollection polygonCollection = new PolygonCollection(execution, batchSize, polygons[i]);
                    polygonCollection.Draw(camera);
                }
            }

            camera = null;
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;

            return this;
        }
    }
}
