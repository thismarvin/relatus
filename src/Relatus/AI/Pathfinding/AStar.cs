using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.AI.Pathfinding
{
    public class AStar
    {
        public static List<Node> AStarAlgorithm(Node start, Node end, Func<Node, Node, int> heuristic)
        {
            HashSet<Node> openSet = new HashSet<Node>() { start };

            while (openSet.Count > 0)
            {
                Node current = MinF(openSet);

                if (current == end)
                {
                    return ReconstructPath(end);
                }

                openSet.Remove(current);

                foreach (NodeConnection neighbor in current.Neighbors)
                {
                    if (neighbor.Target.Disabled)
                        continue;

                    float tentativeGScore = current.G + neighbor.Weight;

                    if (tentativeGScore < neighbor.Target.G)
                    {
                        neighbor.Target.Previous = current;
                        neighbor.Target.G = tentativeGScore;
                        neighbor.Target.F = neighbor.Target.G + heuristic(current, neighbor.Target);

                        if (!openSet.Contains(neighbor.Target))
                        {
                            openSet.Add(neighbor.Target);
                        }
                    }
                }
            }

            return new List<Node>();

            Node MinF(HashSet<Node> nodes)
            {
                Node min = null;

                foreach (Node node in nodes)
                {
                    min = min == null || node.F < min.F ? node : min;
                }

                return min;
            }

            List<Node> ReconstructPath(Node _start)
            {
                List<Node> result = new List<Node>();

                while (_start != null)
                {
                    result.Add(_start);
                    _start = _start.Previous;
                }

                result.Reverse();

                return result;
            }
        }
    }
}
