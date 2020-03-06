using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.SchemeSimplifying
{
    class SchemeSimplifier
    {
        private Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> _connectionGraph;
        private Dictionary<ElementBase, int> _elementsToDelete;
        private Dictionary<Vector2Int, bool> _markedVertices;
        private Dictionary<Vector2Int, Vector2Int> _vertexTree;
        private Dictionary<Vector2Int, int> _rootConnectionsCount;
        private Dictionary<Vector2Int, ElementBase> _nullorVertices;

        public SchemeSimplifier(Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> connectionGraph)
        {
            _connectionGraph = connectionGraph;
            _elementsToDelete = new Dictionary<ElementBase, int>();
            _vertexTree = new Dictionary<Vector2Int, Vector2Int>();
            _rootConnectionsCount = new Dictionary<Vector2Int, int>();
            _nullorVertices = new Dictionary<Vector2Int, ElementBase>();

            InitMarkedVertices();
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
            var deleteResult = new Dictionary<ElementBase, int>();

            var notMarkedCount = _markedVertices.Count - 4;

            MarkNullorVertices();

            var verticesInIteration = new List<Vector2Int>(_nullorVertices.Keys);
            var deleteStep = 0;
            var someValue = _connectionGraph.Count;

            do
            {
                someValue--;

                var verticesInNextIteration = new List<Vector2Int>();

                foreach (var rootVertex in verticesInIteration)
                {
                    var isNextDeleteStep = false;

                    var graphElementsToDelete = new Dictionary<Vector2Int, Vector2Int>();

                    var isDeleteVertex = false;

                    foreach (var connectedVertex in _connectionGraph[rootVertex].Keys)
                    {
                        var element = _connectionGraph[rootVertex][connectedVertex];

                        switch (element)
                        {
                            case Wire _:
                            {
                                if (!_markedVertices[connectedVertex])
                                {
                                    if (!_vertexTree.ContainsKey(connectedVertex))
                                    {
                                        isDeleteVertex = AddToVertexTree(connectedVertex, rootVertex);
                                    }

                                    if (!verticesInNextIteration.Contains(connectedVertex))
                                        verticesInNextIteration.Add(connectedVertex);

                                    notMarkedCount--;
                                    _markedVertices[connectedVertex] = true;
                                }

                                break;
                            }
                            case Resistor _:
                            {
                                if (!_vertexTree.ContainsKey(connectedVertex))
                                {
                                    isDeleteVertex = AddToVertexTree(connectedVertex, rootVertex);
                                }

                                if (_connectionGraph[rootVertex].Count == 2 && _rootConnectionsCount[_vertexTree[connectedVertex]] == 1)
                                {
                                    if (!deleteResult.ContainsKey(element))
                                        deleteResult.Add(element, deleteStep);
                                    isNextDeleteStep = true;

                                    _rootConnectionsCount[_vertexTree[connectedVertex]]--;

                                    if (!verticesInNextIteration.Contains(connectedVertex) && !_markedVertices[connectedVertex])
                                        verticesInNextIteration.Add(connectedVertex);

                                    if (!_markedVertices[connectedVertex])
                                        notMarkedCount--;
                                    _markedVertices[connectedVertex] = true;
                                }
                                else verticesInNextIteration.Add(rootVertex);
                                break;
                            }
                            case Conductor _:
                                if (_vertexTree.ContainsKey(connectedVertex) &&
                                    !_vertexTree[connectedVertex].Equals(_vertexTree[rootVertex]) &&
                                    HasSameRoot(connectedVertex, rootVertex))
                                {
                                    graphElementsToDelete = RemoveFromGraph(connectedVertex, rootVertex);
                                    var temp = GetFromGraph(graphElementsToDelete, deleteStep);

                                    foreach (var pair in temp.Where(pair => !deleteResult.ContainsKey(pair.Key)))
                                    {
                                        deleteResult.Add(pair.Key, pair.Value);
                                    }

                                    isNextDeleteStep = true;

                                    if (_connectionGraph[connectedVertex].Count == 2)
                                    {
                                        _rootConnectionsCount[_vertexTree[connectedVertex]]--;
                                        _vertexTree.Remove(connectedVertex);
                                    }

                                    if (_connectionGraph[rootVertex].Count == 2)
                                    {
                                        _rootConnectionsCount[_vertexTree[rootVertex]]--;
                                        _vertexTree.Remove(rootVertex);
                                    }
                                }
                                else verticesInNextIteration.Add(rootVertex);
                                break;
                        }
                    }

                    foreach (var pair in graphElementsToDelete)
                    {
                        _connectionGraph[pair.Key].Remove(pair.Value);
                        _connectionGraph[pair.Value].Remove(pair.Key);
                    }

                    if (isNextDeleteStep)
                        deleteStep++;

                    if (!isDeleteVertex) continue;
                    if (!_vertexTree.ContainsKey(rootVertex)) continue;
                    _rootConnectionsCount[_vertexTree[rootVertex]]--;
                    _vertexTree.Remove(rootVertex);
                }

                verticesInIteration = verticesInNextIteration;
            } while (notMarkedCount > 0 && verticesInIteration.Count > 0 && someValue > 0);

            return deleteResult;
        }

        private Dictionary<ElementBase, int> GetFromGraph(Dictionary<Vector2Int, Vector2Int> graphElementsToDelete, int deleteStep)
        {
            var result = new Dictionary<ElementBase, int>();

            foreach (var pair in graphElementsToDelete.Where(pair => !result.ContainsKey(_connectionGraph[pair.Key][pair.Value])))
            {
                result.Add(_connectionGraph[pair.Key][pair.Value], deleteStep);
            }

            return result;
        }

        private Dictionary<Vector2Int, Vector2Int> RemoveFromGraph(Vector2Int connectedVertex, Vector2Int rootVertex)
        {
            var result = new Dictionary<Vector2Int, Vector2Int>
            {
                {connectedVertex, rootVertex}, {rootVertex, connectedVertex}
            };

            var rootPrev = connectedVertex;
            var connectedPrev = rootVertex;

            if (_connectionGraph[connectedVertex].Count == 2)
            {
                var isEnd = false;
                while (!isEnd)
                {
                    foreach (var vertex in _connectionGraph.Keys)
                    {
                        if (_connectionGraph[vertex].ContainsKey(connectedVertex) &&
                            !vertex.Equals(connectedPrev) &&
                            _connectionGraph[vertex].Count == 2)
                        {
                            if (!result.ContainsKey(connectedVertex))
                                result.Add(connectedVertex, vertex);
                            if (!result.ContainsKey(vertex))
                                result.Add(vertex, connectedVertex);

                            connectedPrev = connectedVertex;
                            connectedVertex = vertex;
                        }
                        else if (_connectionGraph[vertex].ContainsKey(connectedVertex) &&
                                 !vertex.Equals(connectedPrev) &&
                                 _connectionGraph[vertex].Count > 2)
                        {
                            if (!result.ContainsKey(connectedVertex))
                                result.Add(connectedVertex, vertex);
                            if (!result.ContainsKey(vertex))
                                result.Add(vertex, connectedVertex);

                            isEnd = true;
                        }
                    }
                }
            }

            if (_connectionGraph[rootVertex].Count == 2)
            {
                var isEnd = false;
                while (!isEnd)
                {
                    foreach (var vertex in _connectionGraph.Keys)
                    {
                        if (_connectionGraph[vertex].ContainsKey(rootVertex) &&
                            !vertex.Equals(rootPrev) &&
                            _connectionGraph[vertex].Count == 2)
                        {
                            if (!result.ContainsKey(rootVertex))
                                result.Add(rootVertex, vertex);
                            if (!result.ContainsKey(vertex))
                                result.Add(vertex, rootVertex);

                            rootPrev = rootVertex;
                            rootVertex = vertex;
                        }
                        else if (_connectionGraph[vertex].ContainsKey(rootVertex) &&
                                 !vertex.Equals(rootPrev) &&
                                 _connectionGraph[vertex].Count > 2)
                        {
                            if (!result.ContainsKey(rootVertex))
                                result.Add(rootVertex, vertex);
                            if (!result.ContainsKey(vertex))
                                result.Add(vertex, rootVertex);

                            isEnd = true;
                        }
                    }
                }
            }

            return result;
        }

        private bool HasSameRoot(Vector2Int connectedVertex, Vector2Int rootVertex)
        {
            var root1 = _vertexTree[connectedVertex];
            var root2 = _vertexTree[rootVertex];

            return _nullorVertices[root1] == _nullorVertices[root2];
        }

        private bool AddToVertexTree(Vector2Int connectedVertex, Vector2Int rootVertex)
        {
            if (_vertexTree.ContainsKey(rootVertex))
            {
                _vertexTree.Add(connectedVertex, _vertexTree[rootVertex]);
                _rootConnectionsCount[_vertexTree[rootVertex]]++;
                return true;
            }

            _vertexTree.Add(connectedVertex, rootVertex);
            _rootConnectionsCount[rootVertex]++;
            return false;
        }

        private void MarkNullorVertices()
        {
            var nullator = Scheme.GetNullator();
            var norator = Scheme.GetNorator();

            var nullatorLeftVertex = ConnectionsMaker.GetConnectPosition(true, nullator);
            var nullatorRightVertex = ConnectionsMaker.GetConnectPosition(false, nullator);

            var noratorLeftVertex = ConnectionsMaker.GetConnectPosition(true, norator);
            var noratorRightVertex = ConnectionsMaker.GetConnectPosition(false, norator);

            _nullorVertices.Add(nullatorLeftVertex, nullator);
            _nullorVertices.Add(nullatorRightVertex, nullator);
            _nullorVertices.Add(noratorLeftVertex, norator);
            _nullorVertices.Add(noratorRightVertex, norator);

            _markedVertices[nullatorLeftVertex] = true;
            _markedVertices[nullatorRightVertex] = true;
            _markedVertices[noratorLeftVertex] = true;
            _markedVertices[noratorRightVertex] = true;

            _rootConnectionsCount.Add(nullatorLeftVertex, 0);
            _rootConnectionsCount.Add(nullatorRightVertex, 0);
            _rootConnectionsCount.Add(noratorLeftVertex, 0);
            _rootConnectionsCount.Add(noratorRightVertex, 0);
        }


        private void InitMarkedVertices()
        {
            _markedVertices = new Dictionary<Vector2Int, bool>();

            foreach (var vertex in _connectionGraph.Keys)
            {
                _markedVertices.Add(vertex, false);
            }
        }
    }
}
