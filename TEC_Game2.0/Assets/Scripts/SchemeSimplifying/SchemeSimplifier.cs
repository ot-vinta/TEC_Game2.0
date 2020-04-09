using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.SchemeSimplifying
{
    class SchemeSimplifier
    {
        private Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> _connectionGraph;
        private Dictionary<Vertex, Dictionary<Vertex, List<ElementBase>>> _graph;
        private Dictionary<ElementBase, List<Vertex>> _connectionsToElement;
        private Dictionary<ElementBase, int> _elementsToDelete;
        private Dictionary<Vertex, bool> _markedVertices;
        private readonly Dictionary<Vertex, List<Vertex>> _rootConnections;
        private readonly List<Vertex> _nullorVertices;

        private Dictionary<Vector2Int, bool> _markedVerticesInOldGraph;

        public SchemeSimplifier(Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> connectionGraph)
        {
            _connectionGraph = connectionGraph;
            _elementsToDelete = new Dictionary<ElementBase, int>();
            _rootConnections = new Dictionary<Vertex, List<Vertex>>();
            _nullorVertices = new List<Vertex>();
        }

        public Dictionary<int, List<ElementBase>> Simplify()
        {
            SimplifyGraph();
            InitMarkedVertices();
            InitNullorVertices();

            _elementsToDelete = GetElementsToDelete();
            AddWiresToDelete();

            var result = new Dictionary<int, List<ElementBase>>();

            foreach (var elementWithTiming in _elementsToDelete)
            {
                if (result.ContainsKey(elementWithTiming.Value))
                    result[elementWithTiming.Value].Add(elementWithTiming.Key);
                else
                    result.Add(elementWithTiming.Value, new List<ElementBase>{elementWithTiming.Key});
            }

            return result;
        }

        private Dictionary<ElementBase, int> GetElementsToDelete()
        {
            var deleteResult = new Dictionary<ElementBase, int>();
            var verticesInIteration = new List<Vertex>(_nullorVertices);

            var notMarkedCount = _markedVertices.Count;
            var deleteStep = 0;
            var someValue = _graph.Count;

            do
            {
                someValue--;
                deleteStep++;

                var verticesInNextIteration = new List<Vertex>();
                var graphElementsToDelete = new List<ElementBase>();

                foreach (var rootVertex in verticesInIteration)
                {
                    foreach (var connectedVertex in _graph[rootVertex].Keys)
                    {

                        foreach (var element in _graph[rootVertex][connectedVertex])
                        {
                            switch (element)
                            {
                                case Resistor _:
                                    if (ConnectedOnlyResistorAndNullor(rootVertex) && !IsConnectedParallel(connectedVertex, rootVertex))
                                    {
                                        if (!deleteResult.ContainsKey(element))
                                        {
                                            deleteResult.Add(element, deleteStep);
                                            graphElementsToDelete.Add(element);
                                        }

                                        if (!verticesInNextIteration.Contains(connectedVertex) && !_markedVertices[connectedVertex])
                                            verticesInNextIteration.Add(connectedVertex);

                                        _markedVertices[rootVertex] = true;

                                        if (!_rootConnections.ContainsKey(connectedVertex))
                                        {
                                            _rootConnections.Add(connectedVertex,
                                                _rootConnections.ContainsKey(rootVertex)
                                                    ? _rootConnections[rootVertex]
                                                    : new List<Vertex> {rootVertex});
                                        }
                                        else
                                        {
                                            if (_rootConnections.ContainsKey(rootVertex))
                                            {
                                                foreach (var vertex in _rootConnections[rootVertex].Where(vertex => !_rootConnections[connectedVertex].Contains(vertex)))
                                                {
                                                    _rootConnections[connectedVertex].Add(vertex);
                                                }
                                            }
                                            else
                                            {
                                                _rootConnections[connectedVertex].Add(rootVertex);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!verticesInNextIteration.Contains(rootVertex))
                                            verticesInNextIteration.Add(rootVertex);
                                    }
                                    break;
                                case Conductor _:
                                    if (IsConnectedParallel(connectedVertex, rootVertex))
                                    {
                                        if (!deleteResult.ContainsKey(element))
                                        {
                                            deleteResult.Add(element, deleteStep);
                                            graphElementsToDelete.Add(element);
                                        }

                                        if (_graph[rootVertex].Count <= 2)
                                            _markedVertices[rootVertex] = true;
                                        if (_graph[connectedVertex].Count <= 2)
                                            _markedVertices[connectedVertex] = true;

                                        verticesInNextIteration.AddRange(_graph[connectedVertex].Keys.Where(vertex => !_markedVertices[connectedVertex]));
                                    }
                                    else
                                    {
                                        if (!verticesInNextIteration.Contains(rootVertex))
                                            verticesInNextIteration.Add(rootVertex);
                                    }
                                    break;
                            }
                        }
                    }
                }

                foreach (var elementToDelete in graphElementsToDelete)
                {
                    foreach (var root in _graph.Keys)
                    {
                        foreach (var connected in _graph[root].Keys.Where(connected => _graph[root][connected].Contains(elementToDelete)))
                        {
                            _graph[root][connected].Remove(elementToDelete);
                        }
                    }
                }

                verticesInIteration = verticesInNextIteration;
            } while (notMarkedCount > 0 && verticesInIteration.Count > 0 && someValue > 0);

            return deleteResult;
        }

        private void AddWiresToDelete()
        {
            var result = new Dictionary<ElementBase, int>();
            
            foreach (var pair in _elementsToDelete)
            {
                if (!(pair.Key is Conductor conductor)) continue;
                
                var wires = GetConnectedWires(conductor);

                foreach (var wire in wires.Where(wire => !result.ContainsKey(wire)))
                {
                    result.Add(wire, _elementsToDelete[conductor]);
                }
            }

            foreach (var pair in result)
            {
                _elementsToDelete.Add(pair.Key, pair.Value);
            }
        }

        private List<Wire> GetConnectedWires(ElementBase conductor)
        {
            var markedElements = new List<ElementBase>();

            var result = new List<Wire>();

            var elementsInIteration = new List<ElementBase> {conductor};
            while (elementsInIteration.Count > 0)
            {
                var elementsInNextIteration = new List<Wire>();
                
                foreach (var element in elementsInIteration)
                {
                    markedElements.Add(element);

                    Vector2Int leftPos, rightPos;

                    if (element is Wire temp)
                    {
                        leftPos  = (Vector2Int) temp.pivotPosition;
                        rightPos = (Vector2Int) temp.secondPosition;
                    }
                    else
                    {
                        leftPos = ConnectionsMaker.GetConnectPosition(true, element);
                        rightPos = ConnectionsMaker.GetConnectPosition(false, element);
                    }

                    var tempList = new List<Wire>();
                    
                    if (_connectionGraph[leftPos].Values.Count <= 2)
                        foreach (var connectedElement in _connectionGraph[leftPos].Values)
                        {
                            if (!(connectedElement is Wire) && connectedElement != conductor)
                            {
                                tempList.Clear();
                                break;
                            }
                            if (!markedElements.Contains(connectedElement))
                                tempList.Add((Wire) connectedElement);
                        }
                    
                    elementsInNextIteration.AddRange(tempList);
                    tempList.Clear();
                    
                    if (_connectionGraph[rightPos].Values.Count <= 2)
                        foreach (var connectedElement in _connectionGraph[rightPos].Values)
                        {
                            if (!(connectedElement is Wire) && connectedElement != conductor)
                            {
                                tempList.Clear();
                                break;
                            }
                            if (!markedElements.Contains(connectedElement))
                                tempList.Add((Wire) connectedElement);
                        }
                    
                    elementsInNextIteration.AddRange(tempList);
                }

                elementsInIteration = new List<ElementBase>(elementsInNextIteration);
                result.AddRange(elementsInNextIteration);
            }

            return result;
        }

        private bool ConnectedOnlyResistorAndNullor(Vertex rootVertex)
        {
            var count = 0;
            var nullorCount = 0;
            foreach (var element in _graph[rootVertex].Keys.SelectMany(connectedVertex => _graph[rootVertex][connectedVertex]))
            {
                switch (element)
                {
                    case Conductor _:
                        return false;
                    case Resistor _:
                        count++;
                        break;
                    case Nullator _:
                        nullorCount++;
                        break;
                    case Norator _:
                        nullorCount++;
                        break;
                }
            }

            if (nullorCount == 2)
                return false;
            return count == 1;
        }

        private bool IsConnectedParallel(Vertex connectedVertex, Vertex rootVertex)
        {
            var nullator = Scheme.GetNullator();
            var norator = Scheme.GetNorator();

            var vertex1List = _rootConnections.ContainsKey(connectedVertex)
                ? _rootConnections[connectedVertex]
                : new List<Vertex>();
            var vertex2List = _rootConnections.ContainsKey(rootVertex) 
                ? _rootConnections[rootVertex] 
                : new List<Vertex>();

            vertex1List.Add(connectedVertex);
            vertex2List.Add(rootVertex);

            var result = false;
            var markedElements = new List<ElementBase>();
            var count = 0;
            foreach (var element in from elements in _graph[connectedVertex].Values 
                                    from element in elements where !markedElements.Contains(element) select element)
            {
                markedElements.Add(element);
                count++;
            }
            foreach (var element in from elements in _graph[rootVertex].Values 
                                    from element in elements where !markedElements.Contains(element) select element)
            {
                markedElements.Add(element);
                count++;
            }

            if (count > 2)
                result = true;

            return result && (from vertex1 in vertex1List 
                              from vertex2 in vertex2List 
                              where vertex1 != vertex2 && 
                                    (_connectionsToElement[nullator].Contains(vertex1) && _connectionsToElement[nullator].Contains(vertex2) || 
                                     _connectionsToElement[norator].Contains(vertex1) && _connectionsToElement[norator].Contains(vertex2)) select vertex1).Any();
        }

        private void InitNullorVertices()
        {
            var nullator = Scheme.GetNullator();
            var norator = Scheme.GetNorator();

            var nullatorLeftVertex = _connectionsToElement[nullator][0];
            var nullatorRightVertex = _connectionsToElement[nullator][1];

            var noratorLeftVertex = _connectionsToElement[norator][0];
            var noratorRightVertex = _connectionsToElement[norator][1];

            if (!_nullorVertices.Contains(nullatorLeftVertex))
                _nullorVertices.Add(nullatorLeftVertex);

            if (!_nullorVertices.Contains(nullatorRightVertex))
                _nullorVertices.Add(nullatorRightVertex);

            if (!_nullorVertices.Contains(noratorLeftVertex))
                _nullorVertices.Add(noratorLeftVertex);

            if (!_nullorVertices.Contains(noratorRightVertex))
                _nullorVertices.Add(noratorRightVertex);
        }

        private void InitMarkedVertices()
        {
            _markedVertices = new Dictionary<Vertex, bool>();

            foreach (var vertex in _graph.Keys)
            {
                _markedVertices.Add(vertex, false);
            }
        }

        private void SimplifyGraph()
        {
            var connectedByWiresVertices = new Dictionary<Vector2Int, List<Vector2Int>>();
            _markedVerticesInOldGraph = _connectionGraph.Keys.ToDictionary(vertex => vertex, vertex => false);

            foreach (var vertex in _connectionGraph.Keys.Where(vertex => HasChainElement(vertex) && !_markedVerticesInOldGraph[vertex]))
            {
                connectedByWiresVertices.Add(vertex, new List<Vector2Int>());
                _markedVerticesInOldGraph[vertex] = true;

                foreach (var connected in _connectionGraph[vertex].Keys
                    .Where(connected => _connectionGraph[vertex][connected] is Wire)
                    .SelectMany(FindConnectedVertex))
                {
                    connectedByWiresVertices[vertex].Add(connected);
                }
            }

            CreateSimplifiedGraph(connectedByWiresVertices);
        }

        private void CreateSimplifiedGraph(Dictionary<Vector2Int, List<Vector2Int>> connectedByWiresVertices)
        {
            _graph = new Dictionary<Vertex, Dictionary<Vertex, List<ElementBase>>>();
            _connectionsToElement = new Dictionary<ElementBase, List<Vertex>>();

            var count = 1;
            foreach (var pair in connectedByWiresVertices)
            {
                var vertex = new Vertex(count);
                count++;

                vertex.AddPosition(pair.Key);
                foreach (var pos in pair.Value)
                {
                    vertex.AddPosition(pos);
                }

                _graph.Add(vertex, new Dictionary<Vertex, List<ElementBase>>());
            }

            foreach (var vertex in _graph.Keys)
            {
                foreach (var pos in vertex.positions)
                {
                    foreach (var connected in _connectionGraph[pos].Keys)
                    {
                        if (_connectionGraph[pos][connected] is Wire) continue;

                        foreach (var connectedVertex in _graph.Keys.Where(connectedVertex => connectedVertex.positions.Contains(connected)))
                        {
                            if (connectedVertex != vertex)
                            {
                                if (!_graph[vertex].ContainsKey(connectedVertex))
                                {
                                    _graph[vertex].Add(connectedVertex,
                                        new List<ElementBase> {_connectionGraph[pos][connected]});
                                }
                                else if (!_graph[vertex][connectedVertex].Contains(_connectionGraph[pos][connected]))
                                    _graph[vertex][connectedVertex].Add(_connectionGraph[pos][connected]);

                                if (!_graph[connectedVertex].ContainsKey(vertex))
                                {
                                    _graph[connectedVertex].Add(vertex,
                                        new List<ElementBase> {_connectionGraph[pos][connected]});
                                }
                                else if (!_graph[connectedVertex][vertex].Contains(_connectionGraph[pos][connected]))
                                    _graph[connectedVertex][vertex].Add(_connectionGraph[pos][connected]);
                            }

                            if (!_connectionsToElement.ContainsKey(_connectionGraph[pos][connected]))
                                _connectionsToElement.Add(_connectionGraph[pos][connected], new List<Vertex> { vertex });
                            else _connectionsToElement[_connectionGraph[pos][connected]].Add(vertex);

                            if (!_connectionsToElement.ContainsKey(_connectionGraph[pos][connected]))
                                _connectionsToElement.Add(_connectionGraph[pos][connected], new List<Vertex> { connectedVertex });
                            else _connectionsToElement[_connectionGraph[pos][connected]].Add(connectedVertex);
                        }
                    }
                }
            }
        }

        private List<Vector2Int> FindConnectedVertex(Vector2Int connected)
        {
            _markedVerticesInOldGraph[connected] = true;

            var result = new List<Vector2Int> {connected};

            result.AddRange(_connectionGraph[connected].Keys
                                        .Where(newConnected => !_markedVerticesInOldGraph[newConnected] && 
                                                               _connectionGraph[connected][newConnected] is Wire)
                                        .SelectMany(FindConnectedVertex));

            return result;
        }

        private bool HasChainElement(Vector2Int vertex)
        {
            return _connectionGraph[vertex].Values.Any(element => !(element is Wire));
        }
    }
}