using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.AI.Pathfinding
{
    public class NodeConnection
    {
        public Node Target { get; set; }
        public float Weight { get; set; }

        public NodeConnection(Node target, float weight)
        {
            Target = target;
            Weight = weight;
        }
    }
}
