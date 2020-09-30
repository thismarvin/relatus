using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class GeometryBatcher : Batcher<Geometry>
    {
        private readonly List<List<Geometry>> polygons;
        private int index;

        private static readonly GraphicsDevice graphicsDevice;

        static GeometryBatcher()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        internal GeometryBatcher()
        {
            polygons = new List<List<Geometry>>();
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;
        }

        public override Batcher<Geometry> Begin()
        {
            polygons.Clear();

            polygons.Add(new List<Geometry>((int)batchSize));
            index = 0;

            return this;
        }

        public override Batcher<Geometry> Add(Geometry polygon)
        {
            polygons[^1].Add(polygon);
            index++;

            if (index >= batchSize)
            {
                polygons.Add(new List<Geometry>((int)batchSize));
                index = 0;
            }

            return this;
        }

        public override Batcher<Geometry> End()
        {
            if (camera == null)
                throw new RelatusException("A Camera has not been attached yet. Make sure to call AttachCamera(camera).", new ArgumentNullException());

            for (int i = 0; i < polygons.Count; i++)
            {
                if (i + 1 == polygons.Count)
                {
                    using GeometryCollection polygonCollection = new GeometryCollection(execution, (uint)index, polygons[i]);
                    polygonCollection.Draw(camera);
                }
                else
                {
                    using GeometryCollection polygonCollection = new GeometryCollection(execution, batchSize, polygons[i]);
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
