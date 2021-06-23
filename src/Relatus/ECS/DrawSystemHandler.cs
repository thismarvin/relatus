using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    internal class DrawSystemHandler
    {
        private readonly MorroSystem parent;
        private readonly Action<uint, Camera> onDraw;

        public DrawSystemHandler(MorroSystem parent, Action<uint, Camera> onDraw)
        {
            this.parent = parent;
            this.onDraw = onDraw;
        }

        public void Draw(Camera camera)
        {
            foreach (uint entity in parent.Entities)
            {
                onDraw(entity, camera);
            }
        }
    }
}
