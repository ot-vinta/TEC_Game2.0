using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts
{
    class ConnectionsMaker
    {
        private static Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> g;
        private static List<ElementBase> schemeElements;

        public static Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> MakeConnectionGraph()
        {
            schemeElements = new List<ElementBase>(Scheme.elements.Values);
            g = new Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>>();

            List<Vector2Int> connections = new List<Vector2Int>();

            foreach (var element in Scheme.GetNullorElementsList())
            {
                Vector2Int leftPos = GetConnectPosition(true, element);
                Vector2Int rightPos = GetConnectPosition(false, element);

                connections.Add(leftPos);
                connections.Add(rightPos);

                g.Add(leftPos, new Dictionary<Vector2Int, ElementBase>());
                g.Add(rightPos, new Dictionary<Vector2Int, ElementBase>());
                g[leftPos].Add(rightPos, element);
                g[rightPos].Add(leftPos, element);

                schemeElements.Remove(element);
            }

            NextIteration(connections);

            return g;
        }

        private static void NextIteration(List<Vector2Int> connections)
        {
            List<Vector2Int> connectionsForNextIteration = new List<Vector2Int>();

            foreach (var element in schemeElements)
            {
                Vector2Int leftPos, rightPos;

                if (element is Wire)
                {
                    Wire wire = (Wire)element;
                    leftPos = new Vector2Int(wire.pivotPosition.x, wire.pivotPosition.y);
                    rightPos = new Vector2Int(wire.secondPosition.x, wire.secondPosition.y);
                }
                else
                {
                    leftPos = GetConnectPosition(true, element);
                    rightPos = GetConnectPosition(false, element);
                }

                foreach (var position in connections)
                {
                    if (position.Equals(leftPos))
                    {
                        if (!g[leftPos].ContainsKey(rightPos))
                            g[leftPos].Add(rightPos, element);

                        if (g.ContainsKey(rightPos) && !g[rightPos].ContainsKey(leftPos))
                            g[rightPos].Add(leftPos, element);
                        else if (!g.ContainsKey(rightPos))
                        {
                            g.Add(rightPos, new Dictionary<Vector2Int, ElementBase>());
                            g[rightPos].Add(leftPos, element);

                            connectionsForNextIteration.Add(rightPos);
                        }
                    }

                    if (position.Equals(rightPos))
                    {
                        if (!g[rightPos].ContainsKey(leftPos))
                            g[rightPos].Add(leftPos, element);

                        if (g.ContainsKey(leftPos) && !g[leftPos].ContainsKey(rightPos))
                            g[leftPos].Add(rightPos, element);
                        else if (!g.ContainsKey(leftPos))
                        {
                            g.Add(leftPos, new Dictionary<Vector2Int, ElementBase>());
                            g[leftPos].Add(rightPos, element);

                            connectionsForNextIteration.Add(leftPos);
                        }
                    }
                }
            }

            if (connectionsForNextIteration.Count > 0)
                NextIteration(connectionsForNextIteration);
        }

        private static Vector2Int GetConnectPosition(bool isLeft, ElementBase element)
        {
            if (isLeft)
            {
                if (element.angle == 0 || element.angle == 180)
                    return new Vector2Int(element.pivotPosition.x + 2 * (element.angle / 90 - 1), element.pivotPosition.y);
                else
                    return new Vector2Int(element.pivotPosition.x, element.pivotPosition.y + 2 * (element.angle / 90 - 2));
            }
            else
            {
                if (element.angle == 0 || element.angle == 180)
                    return new Vector2Int(element.pivotPosition.x - 2 * (element.angle / 90 - 1), element.pivotPosition.y);
                else
                    return new Vector2Int(element.pivotPosition.x, element.pivotPosition.y - 2 * (element.angle / 90 - 2));
            }
        }
    }
}
