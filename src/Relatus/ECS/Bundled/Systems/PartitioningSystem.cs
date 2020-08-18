using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
{
    public abstract class PartitioningSystem : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;

        protected Partitioner<PartitionerEntry> partitioner;

        public PartitioningSystem(MorroFactory factory) : base(factory)
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

        public override void UpdateEntity(uint entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];

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
            public RectangleF Span { get; set; }

            public PartitionerEntry(uint entity, CPosition position, CDimension dimension)
            {
                Identifier = (int)entity;
                Span = new RectangleF(position.X, position.Y, dimension.Width, dimension.Height);
            }
        }
    }
}
