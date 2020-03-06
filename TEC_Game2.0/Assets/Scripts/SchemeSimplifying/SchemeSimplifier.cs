using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.SchemeSimplifying
{
    class SchemeSimplifier
    {
        private Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> _connectionGraph;
        private Dictionary<ElementBase, int> _elementsToDelete;
        private Dictionary<TreeElement, List<TreeElement>> _vertexTree;
        private List<Vector2Int> _nullorVertices;
        private List<TreeElement> _verticesToDelete;

        private Dictionary<TreeElement, TreeElement> _connectedVerticesWithoutWires;

        public SchemeSimplifier(Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> connectionGraph)
        {
            _connectionGraph = connectionGraph;
            _elementsToDelete = new Dictionary<ElementBase, int>();
            _vertexTree = new Dictionary<TreeElement, List<TreeElement>>();
            _nullorVertices = new List<Vector2Int>();

            _connectedVerticesWithoutWires = new Dictionary<TreeElement, TreeElement>();
        }

        public Dictionary<int, List<ElementBase>> Simplify()
        {
            _elementsToDelete = GetElementsToDelete();

            var result = new Dictionary<int, List<ElementBase>>();

            foreach (var elementWithTiming in _elementsToDelete)
            {
                if (result.ContainsKey(elementWithTiming.Value))
                    result[elementWithTiming.Value].Add(elementWithTiming.Key);
                else
                    result.Add(elementWithTiming.Value, new List<ElementBase>(){elementWithTiming.Key});
            }

            return result;
        }

        private Dictionary<ElementBase, int> GetElementsToDelete()
        {
            var result = new Dictionary<ElementBase, int>();

            MakeVertexTree();

            var verticesInIteration = new List<TreeElement>();
            var verticesInNextIteration = new List<TreeElement>();
            var list = _vertexTree.Keys.ToList();
            for (var i = 2; i < 6; i++)
            {
                verticesInIteration.Add(list[i]);
            }

            _verticesToDelete = new List<TreeElement>();

            while (verticesInIteration.Count > 0)
            {
                foreach (var rootVertex in verticesInIteration)
                {
                    foreach (var connectedVertex in _vertexTree[rootVertex])
                    {
                        var element = connectedVertex.ElementToRoot;

                        if (element is Resistor &&
                            _vertexTree[rootVertex].Count == 1)
                        {
                            if (!_elementsToDelete.ContainsKey(element))
                                _elementsToDelete.Add(element, GetTime(connectedVertex, element));

                            if (!verticesInNextIteration.Contains(connectedVertex))
                                verticesInNextIteration.Add(connectedVertex);
                        }
                        else if (element is Conductor)
                        {
                            var secondConductorVertex = connectedVertex.Root;
                            if (VertexTreeHasSecondVertex(secondConductorVertex, connectedVertex))
                            {
                                if (!_elementsToDelete.ContainsKey(element))
                                    _elementsToDelete.Add(element, Math.Max(GetTime(connectedVertex, element), 
                                                                        GetTime(secondConductorVertex, element)));
                                DeleteBranch(secondConductorVertex, connectedVertex);
                            }
                        }


                    }
                }

                verticesInIteration = new List<TreeElement>(verticesInNextIteration);
                verticesInNextIteration.Clear();
            }

            return result;
        }

        private void DeleteBranch(TreeElement secondConductorVertex, TreeElement firstConductorVertex)
        {
            var deleteResult = new List<TreeElement>();

            deleteResult.Add(secondConductorVertex);
            deleteResult.Add(firstConductorVertex);

            
        }

        private bool VertexTreeHasSecondVertex(TreeElement secondConductorVertex, TreeElement firstConductorVertex)
        {
            return _vertexTree.Keys.Any(vertex => vertex.Position.Equals(secondConductorVertex.Position) && HasSameRoot(secondConductorVertex, firstConductorVertex));
        }

        private static bool HasSameRoot(TreeElement secondConductorVertex, TreeElement firstConductorVertex)
        {
            while (firstConductorVertex.Root != null)
            {
                firstConductorVertex = firstConductorVertex.Root;
            }

            var root = firstConductorVertex;

            while (secondConductorVertex.Root != null)
            {
                secondConductorVertex = secondConductorVertex.Root;
            }

            return root.Position.Equals(secondConductorVertex.Position);
        }


        /*
            TODO This method should return the time, when element will be deleted(for delete animation)
            For now it's just returns position of element in vertexTree
        */
        private static int GetTime(TreeElement vertex, ElementBase element)
        {
            var resultTime = 0;

            while (vertex.Root != null)
            {
                vertex = vertex.Root;
                resultTime++;
            }

            return resultTime;
        }

        private void MakeVertexTree()
        {
            _vertexTree.Clear();
            _nullorVertices.Clear();

            DoNextIteration(InitFirstVertices());
        }

        private void DoNextIteration(List<TreeElement> verticesInIteration)
        {
            if (verticesInIteration.Count <= 0) return;

            var verticesInNextIteration = new List<TreeElement>();

            foreach (var rootVertex in verticesInIteration)
            {
                foreach (var connectedVertex in _connectionGraph[rootVertex.Position].Keys)
                {
                    if (_nullorVertices.Contains(connectedVertex) ||
                        rootVertex.Root.Position.Equals(connectedVertex)) continue;

                    var newElement = new TreeElement(connectedVertex, rootVertex, _connectionGraph[connectedVertex][rootVertex.Position]);
                    if (_vertexTree.ContainsKey(newElement) && _vertexTree[newElement].Contains(rootVertex))
                        continue;

                    List<TreeElement> elementsWithSamePosition;

                    //Skip wires. It's useless
                    if (_connectionGraph[rootVertex.Position][connectedVertex] is Wire)
                    {
                        if (_connectedVerticesWithoutWires.ContainsKey(rootVertex))
                        {
                            var temp = _connectedVerticesWithoutWires[rootVertex];
                            _connectedVerticesWithoutWires.Add(newElement, temp);
                        }
                        else
                        {
                            _connectedVerticesWithoutWires.Add(newElement, rootVertex);
                        }

                        elementsWithSamePosition = _vertexTree.Keys.Where(element => element.Position.Equals(newElement.Position) && !Equals(element, newElement)).ToList();
                        elementsWithSamePosition.Add(newElement);

                        if (!HasOneRoot(elementsWithSamePosition))
                        {
                            verticesInNextIteration.Add(newElement);
                        }
                        
                        continue;
                    }

                    if (_connectedVerticesWithoutWires.ContainsKey(rootVertex))
                    {
                        if (!_vertexTree.ContainsKey(_connectedVerticesWithoutWires[rootVertex]))
                        {
                            _vertexTree.Add(_connectedVerticesWithoutWires[rootVertex], new List<TreeElement>());

                        }

                        _vertexTree[_connectedVerticesWithoutWires[rootVertex]].Add(newElement);
                        _vertexTree.Add(newElement, new List<TreeElement>());

                    }
                    else
                    {
                        _vertexTree[rootVertex].Add(newElement);
                        _vertexTree.Add(newElement, new List<TreeElement>());
                    }

                    elementsWithSamePosition = _vertexTree.Keys.Where(element => element.Position.Equals(newElement.Position) && !Equals(element, newElement)).ToList();
                    elementsWithSamePosition.Add(newElement);

                    if (!HasOneRoot(elementsWithSamePosition))
                    {
                        verticesInNextIteration.Add(newElement);
                    }
                }
            }

            DoNextIteration(verticesInNextIteration);
        }

        private List<TreeElement> InitFirstVertices()
        {
            var result = new List<TreeElement>();

            var list = new List<Vector2Int>(_connectionGraph.Keys);

            var nullor = Scheme.GetNullorElementsList();

            var nullatorRoot = new TreeElement((Vector2Int)Scheme.GetNullator().pivotPosition, null, Scheme.GetNullator());
            var noratorRoot = new TreeElement((Vector2Int)Scheme.GetNorator().pivotPosition, null, Scheme.GetNorator());

            _vertexTree.Add(nullatorRoot, new List<TreeElement>());
            _vertexTree.Add(noratorRoot, new List<TreeElement>());

            for (var i = 0; i < 4; i++)
            {
                if (nullor[i / 2] is Nullator)
                {
                    var newElement = new TreeElement(list[i], nullatorRoot, nullatorRoot.ElementToRoot);

                    _vertexTree.Add(newElement, new List<TreeElement>());
                    _vertexTree[nullatorRoot].Add(newElement);

                    result.Add(newElement);
                }
                else
                {
                    var newElement = new TreeElement(list[i], noratorRoot, noratorRoot.ElementToRoot);

                    _vertexTree.Add(newElement, new List<TreeElement>());
                    _vertexTree[noratorRoot].Add(newElement);

                    result.Add(newElement);
                }
                _nullorVertices.Add(list[i]);
            }

            return result;
        }

        private static bool HasOneRoot(List<TreeElement> elements)
        {
            var elementsWay = new List<TreeElement>();
            var element = elements[elements.Count - 1];
            elements.Remove(element);

            while (element.Root != null)
            {
                elementsWay.Add(element.Root);
                element = element.Root;
            }

            foreach (var elem in elements)
            {
                var temp = elem;
                while (temp.Root != null)
                {
                    if (elementsWay.Contains(temp.Root))
                        return true;
                    temp = temp.Root;
                }
            }

            return false;
        }
    }
}
