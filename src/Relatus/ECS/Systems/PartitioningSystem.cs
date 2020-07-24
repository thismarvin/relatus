using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public abstract class PartitioningSystem : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;

        protected Partitioner<PartitionerEntry> partitioner;

        internal PartitioningSystem(MorroFactory factory, int targetFPS) : base(factory, 0, targetFPS)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CPartitionable));
        }

        public List<int> Query(RectangleF bounds)
        {
            return partitioner.Query(bounds);
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];

            partitioner.Add(new PartitionerEntry(entity, position, dimension));
        }

        public override void Update()
        {
            partitioner.Clear();

            positions = factory.GetData<CPosition>();
            dimensions = factory.GetData<CDimension>();

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
