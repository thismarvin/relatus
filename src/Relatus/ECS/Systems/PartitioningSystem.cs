using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public abstract class PartitioningSystem : UpdateSystem
    {
        private CPosition[] positions;
        private CDimension[] dimensions;

        protected Partitioner<PartitionerEntry> partitioner;

        internal PartitioningSystem(MorroFactory factory, int targetFPS) : base(factory, 0, targetFPS)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CPartitionable));
        }

        public List<int> Query(RectangleF bounds)
        {
            return partitioner.Query(bounds);
        }

        public List<int> Query(RectangleF bounds, int buffer)
        {
            return partitioner.Query(new RectangleF(bounds.X - buffer, bounds.Y + buffer, bounds.Width + buffer * 2, bounds.Height + buffer * 2));
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = positions[entity];
            CDimension dimension = dimensions[entity];

            partitioner.Add(new PartitionerEntry(entity, position, dimension));
        }

        public override void Update()
        {
            partitioner.Clear();

            positions = positions ?? factory.GetData<CPosition>();
            dimensions = dimensions ?? factory.GetData<CDimension>();

            base.Update();
        }

        protected class PartitionerEntry : IPartitionable
        {
            public int Identifier { get; set; }
            public RectangleF Bounds { get; set; }

            public PartitionerEntry(int entity, CPosition position, CDimension dimension)
            {
                Identifier = entity;
                Bounds = new RectangleF(position.X, position.Y, dimension.Width, dimension.Height);
            }
        }
    }
}
