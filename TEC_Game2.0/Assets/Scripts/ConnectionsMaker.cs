using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class ConnectionsMaker
    {
        private static Dictionary<Vector3Int, List<Vector3Int>> _marked;
        private static List<Wire> wiresToCheck;
        private static Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> g;
        private static List<ElementBase> _schemeElements;

        public static Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> MakeConnectionGraph()
        {
            _schemeElements = new List<ElementBase>(Scheme.elements.Values);
            g = new Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>>();
            _marked = new Dictionary<Vector3Int, List<Vector3Int>>();

            var connections = new List<Vector2Int>();

            foreach (var element in Scheme.GetNullorElementsList())
            {
                var leftPos = GetConnectPosition(true, element);
                var rightPos = GetConnectPosition(false, element);

                connections.Add(leftPos);
                connections.Add(rightPos);

                g.Add(leftPos, new Dictionary<Vector2Int, ElementBase>());
                g.Add(rightPos, new Dictionary<Vector2Int, ElementBase>());
                g[leftPos].Add(rightPos, element);
                g[rightPos].Add(leftPos, element);

                _schemeElements.Remove(element);
            }
            
            wiresToCheck = new List<Wire>();

            NextIteration(connections);

            CheckWires();

            return g;
        }

        private static void NextIteration(List<Vector2Int> connections)
        {
            var connectionsForNextIteration = new List<Vector2Int>();

            foreach (var element in _schemeElements)
            {
                Vector2Int leftPos, rightPos;
                var isAddedToGraph = false;

                if (element is Wire wire)
                {
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
                        {
                            g[leftPos].Add(rightPos, element);
                            isAddedToGraph = true;
                        }

                        if (g.ContainsKey(rightPos) && !g[rightPos].ContainsKey(leftPos))
                        {
                            g[rightPos].Add(leftPos, element);
                            isAddedToGraph = true;
                        }
                        else if (!g.ContainsKey(rightPos))
                        {
                            g.Add(rightPos, new Dictionary<Vector2Int, ElementBase>());
                            g[rightPos].Add(leftPos, element);

                            connectionsForNextIteration.Add(rightPos);
                            isAddedToGraph = true;
                        }
                    }

                    if (!position.Equals(rightPos)) continue;
                    if (!g[rightPos].ContainsKey(leftPos))
                    {
                        g[rightPos].Add(leftPos, element);
                        isAddedToGraph = true;
                    }

                    if (g.ContainsKey(leftPos) && !g[leftPos].ContainsKey(rightPos))
                    {
                        g[leftPos].Add(rightPos, element);
                        isAddedToGraph = true;
                    }
                    else if (!g.ContainsKey(leftPos))
                    {
                        g.Add(leftPos, new Dictionary<Vector2Int, ElementBase>());
                        g[leftPos].Add(rightPos, element);

                        connectionsForNextIteration.Add(leftPos);
                        isAddedToGraph = true;
                    }
                }
                
                if (isAddedToGraph && element is Wire wireToAdd)
                    wiresToCheck.Add(wireToAdd);
            }

            if (connectionsForNextIteration.Count > 0)
                NextIteration(connectionsForNextIteration);
        }

        private static void CheckWires()
        {
            foreach (var pair in wiresToCheck
                .Select(GetConnectedWiresWith)
                .SelectMany(connectedWires => connectedWires))
            {
                foreach (var wire in pair.Value.Where(wire => !_marked.ContainsKey(wire.pivotPosition) || 
                                                              !_marked[wire.pivotPosition].Contains(wire.secondPosition)))
                {
                    if (!_marked.ContainsKey(wire.pivotPosition))
                        _marked.Add(wire.pivotPosition, new List<Vector3Int> {wire.secondPosition});
                    else 
                        _marked[wire.pivotPosition].Add(wire.secondPosition);
                    if (!_marked.ContainsKey(wire.secondPosition))
                        _marked.Add(wire.secondPosition, new List<Vector3Int> {wire.pivotPosition});
                    else 
                        _marked[wire.secondPosition].Add(wire.pivotPosition);

                    g[(Vector2Int) wire.pivotPosition].
                        Remove((Vector2Int) wire.secondPosition);
                    g[(Vector2Int) wire.secondPosition].
                        Remove((Vector2Int) wire.pivotPosition);

                    var z = Scheme.GetWiresCount() + 2;
                    var wireFirstPos  = new Vector3Int(wire.pivotPosition.x, wire.pivotPosition.y, z);
                    var wireSecondPos = new Vector3Int(pair.Key.x          , pair.Key.y          , z);
                    var wire1 = new Wire(wireFirstPos, wireSecondPos, wire.angle);
                    PlaceWire(wireFirstPos, wireSecondPos);
                    Scheme.AddElement(wire1);
                    
                    z = Scheme.GetWiresCount() + 2;
                    wireFirstPos  = new Vector3Int(pair.Key.x           , pair.Key.y           , z);
                    wireSecondPos = new Vector3Int(wire.secondPosition.x, wire.secondPosition.y, z);
                    var wire2 = new Wire(wireFirstPos, wireSecondPos, wire.angle);
                    PlaceWire(wireFirstPos, wireSecondPos);
                    Scheme.AddElement(wire2);
                    
                    RemoveWire(wire);
                    
                    if (!g[(Vector2Int) wire.pivotPosition].ContainsKey((Vector2Int) pair.Key))
                        g[(Vector2Int) wire.pivotPosition ].Add((Vector2Int) pair.Key, wire1);
                    if (!g[(Vector2Int) wire.secondPosition].ContainsKey((Vector2Int) pair.Key))
                        g[(Vector2Int) wire.secondPosition].Add((Vector2Int) pair.Key, wire2);
                    
                    if (!g[(Vector2Int) pair.Key].ContainsKey((Vector2Int) wire.pivotPosition))
                        g[(Vector2Int) pair.Key].Add((Vector2Int) wire.pivotPosition,  wire1);
                    if (!g[(Vector2Int) pair.Key].ContainsKey((Vector2Int) wire.secondPosition))
                        g[(Vector2Int) pair.Key].Add((Vector2Int) wire.secondPosition, wire2);
                }
            }
        }

        private static Dictionary<Vector3Int, List<Wire>> GetConnectedWiresWith(Wire wire)
        {
            var pos1 = wire.pivotPosition;
            var pos2 = wire.secondPosition;

            var result = new Dictionary<Vector3Int, List<Wire>>();

            foreach (var wireToCheck in g.Values.SelectMany(
                graphPair => graphPair.Values.Where(
                    element => element is Wire)).Cast<Wire>())
            {
                if (InCenter(pos1, wireToCheck))
                {
                    if (result.ContainsKey(pos1) && !result[pos1].Contains(wireToCheck))
                        result[pos1].Add(wireToCheck);
                    else if (!result.ContainsKey(pos1))
                    {
                        result.Add(pos1, new List<Wire>());
                        result[pos1].Add(wireToCheck);
                    }
                }
                if (InCenter(pos2, wireToCheck))
                {
                    if (result.ContainsKey(pos2) && !result[pos2].Contains(wireToCheck))
                        result[pos2].Add(wireToCheck);
                    else if (!result.ContainsKey(pos2))
                    {
                        result.Add(pos2, new List<Wire>());
                        result[pos2].Add(wireToCheck);
                    }
                }

                if (InCenter(wireToCheck.pivotPosition, wire))
                {
                    if (result.ContainsKey(wireToCheck.pivotPosition) && !result[wireToCheck.pivotPosition].Contains(wire))
                        result[wireToCheck.pivotPosition].Add(wire);
                    else if (!result.ContainsKey(wireToCheck.pivotPosition))
                    {
                        result.Add(wireToCheck.pivotPosition, new List<Wire>());
                        result[wireToCheck.pivotPosition].Add(wire);
                    }
                }
                if (InCenter(wireToCheck.secondPosition, wire))
                {
                    if (result.ContainsKey(wireToCheck.secondPosition) && !result[wireToCheck.secondPosition].Contains(wire))
                        result[wireToCheck.secondPosition].Add(wire);
                    else if (!result.ContainsKey(wireToCheck.secondPosition))
                    {
                        result.Add(wireToCheck.secondPosition, new List<Wire>());
                        result[wireToCheck.secondPosition].Add(wire);
                    }
                }
            }

            return result;
        }

        private static bool InCenter(Vector3Int pos1, Wire wireToCheck)
        {
            var wireMaxX = Math.Max(wireToCheck.pivotPosition.x, wireToCheck.secondPosition.x);
            var wireMinX = Math.Min(wireToCheck.pivotPosition.x, wireToCheck.secondPosition.x);
            var wireMaxY = Math.Max(wireToCheck.pivotPosition.y, wireToCheck.secondPosition.y);
            var wireMinY = Math.Min(wireToCheck.pivotPosition.y, wireToCheck.secondPosition.y);

            return (pos1.x == wireMaxX && pos1.x == wireMinX && pos1.y > wireMinY && pos1.y < wireMaxY) ||
                   (pos1.y == wireMaxY && pos1.y == wireMinY && pos1.x > wireMinX && pos1.x < wireMaxX);
        }

        private static void PlaceWire(Vector3Int firstPos, Vector3Int secondPos)
        {
            var texture = Resources.Load<Texture2D>("Sprites/HalfWireSprite");
            var elementTile = new Tile
            {
                sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.0f, 0.5f),
                    100,
                    1,
                    SpriteMeshType.Tight,
                    Vector4.zero
                )
            };

            float angle;
            float scale;
            
            if (Math.Abs(secondPos.x - firstPos.x) >
                Math.Abs(secondPos.y - firstPos.y))
            {
                angle = secondPos.x - firstPos.x > 0 ? 0.0f : 180.0f;
                scale = (Math.Abs(secondPos.x - firstPos.x) + 0.01f) / 2;
            }
            else
            {
                angle = secondPos.y - firstPos.y > 0 ? 90.0f : 270.0f;
                scale = (Math.Abs(secondPos.y - firstPos.y) + 0.01f) / 2;
            }

            var rotation = Quaternion.Euler(0, 0, angle);
            var m = elementTile.transform;

            m.SetTRS(Vector3.zero, rotation, new Vector3(scale, 1, 1));
            elementTile.transform = m;
            
            var map = GameObject.Find("Map").GetComponent<Tilemap>();
            map.SetTile(firstPos, elementTile);
        }

        private static void RemoveWire(Wire wire)
        {
            var map = GameObject.Find("Map").GetComponent<Tilemap>();
            map.SetTile(wire.pivotPosition, new Tile());
            Scheme.RemoveElement(wire);
        }

        public static Vector2Int GetConnectPosition(bool isLeft, ElementBase element)
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
