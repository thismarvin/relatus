using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.AI.Pathfinding
{
    public static class AStar
    {
        public static List<Node> Pathfind(Node start, Node end, Func<Node, Node, float> heuristic)
        {
            HashSet<Node> openSet = new HashSet<Node>() { start };
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, float> gScores = new Dictionary<Node, float>();
            Dictionary<Node, float> fScores = new Dictionary<Node, float>();

            gScores[start] = 0;
            fScores[start] = heuristic(start, end);

            while (openSet.Count > 0)
            {
                Node current = MinF(openSet, fScores);

                if (current == end)
                {
                    return ReconstructPath(end, cameFrom);
                }

                openSet.Remove(current);

                foreach (NodeConnection neighbor in current.Neighbors)
                {
                    if (neighbor.Target.Disabled)
                        continue;

                    if (!gScores.ContainsKey(current))
                    {
                        gScores[current] = float.MaxValue;
                    }
                    if (!gScores.ContainsKey(neighbor.Target))
                    {
                        gScores[neighbor.Target] = float.MaxValue;
                    }

                    float tentativeGScore = gScores[current] + neighbor.Weight;

                    if (tentativeGScore < gScores[neighbor.Target])
                    {
                        cameFrom[neighbor.Target] = current;
                        gScores[neighbor.Target] = tentativeGScore;
                        fScores[neighbor.Target] = tentativeGScore + heuristic(current, neighbor.Target);

                        if (!openSet.Contains(neighbor.Target))
                        {
                            openSet.Add(neighbor.Target);
                        }
                    }
                }
            }

            return new List<Node>();

            Node MinF(HashSet<Node> _openSet, Dictionary<Node, float> _fScores)
            {
                Node result = null;
                float min = float.MaxValue;

                foreach (Node node in _openSet)
                {
                    if (_fScores[node] < min)
                    {
                        result = node;
                        min = _fScores[node];
                    }
                }

                return result;
            }

            List<Node> ReconstructPath(Node _start, Dictionary<Node, Node> _cameFrom)
            {
                List<Node> result = new List<Node>() { _start };

                while (_cameFrom.ContainsKey(_start))
                {
                    _start = _cameFrom[_start];
                    result.Add(_start);
                }

                result.Reverse();

                return result;
            }
        }
    }
}
