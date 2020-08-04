using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.AI.Pathfinding
{
    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public HashSet<NodeConnection> Neighbors { get; private set; }
        public bool Disabled { get; set; }

        public Node(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;

            Neighbors = new HashSet<NodeConnection>();
        }

        public Node() : this(0, 0, 0)
        {
        }

        public Node AddNeighbor(params NodeConnection[] connections)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                Neighbors.Add(connections[i]);
            }
            return this;
        }
    }
}
