using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.AI.Pathfinding
{
    public class Node
    {
        public float G { get; set; }
        public float H { get; set; }
        public float F { get; set; }

        public HashSet<NodeConnection> Neighbors { get; private set; }
        public Node Previous { get; set; }

        public bool Disabled { get; set; }

        public Node()
        {
            Neighbors = new HashSet<NodeConnection>();
        }

        public Node AddNeighbor(params NodeConnection[] connections)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                Neighbors.Add(connections[i]);
            }
            return this;
        }

        public Node Reset()
        {
            G = 0;
            H = 0;
            F = 0;
            Previous = null;

            return this;
        }
    }
}
