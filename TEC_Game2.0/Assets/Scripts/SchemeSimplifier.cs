﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class SchemeSimplifier
    {
        private Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> g;
        private Dictionary<ElementBase, int> ElementsToDelete;
        private Dictionary<TreeElement, List<TreeElement>> vertexTree;
        private List<Vector2Int> nullorVertices;
        private GameObject mapObject;
        private Tilemap map;

        private class TreeElement
        {
            public Vector2Int position;
            public TreeElement root;

            public TreeElement(Vector2Int position, TreeElement root)
            {
                this.position = position;
                this.root = root;
            }
        }

        public SchemeSimplifier(Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> g)
        {
            this.g = g;
            ElementsToDelete = new Dictionary<ElementBase, int>();
            vertexTree = new Dictionary<TreeElement, List<TreeElement>>();
            nullorVertices = new List<Vector2Int>();

            mapObject = GameObject.Find("Map");
            map = mapObject.GetComponent<Tilemap>();
        }

        public Dictionary<int, List<ElementBase>> SimplifyAsync()
        {
            Simplify();

            mapObject = GameObject.Find("Map");
            map = mapObject.GetComponent<Tilemap>();

            Dictionary<int, List<ElementBase>> result = new Dictionary<int, List<ElementBase>>();

            foreach (var elementWithTiming in ElementsToDelete)
            {
                if (result.ContainsKey(elementWithTiming.Value))
                    result[elementWithTiming.Value].Add(elementWithTiming.Key);
                else
                    result.Add(elementWithTiming.Value, new List<ElementBase>(){elementWithTiming.Key});
            }

            return result;
        }

        private void Simplify()
        {
            ElementsToDelete.Clear();

            MakeVertexTree();

            var verticesInIteration = new List<TreeElement>();
            var verticesInNextIteration = new List<TreeElement>();
            var list = vertexTree.Keys.ToList();
            for (var i = 0; i < 4; i++)
            {
                verticesInIteration.Add(list[i]);
            }

            var stepCount = 0;
            while (verticesInIteration.Count > 0)
            {
                foreach (var vertex in verticesInIteration)
                {
                    if (vertexTree[vertex].Count == 1)
                    {
                        if (!(g[vertex.position][vertexTree[vertex][0].position] is Conductor))
                        {
                            verticesInNextIteration.Add(vertexTree[vertex][0]);
                            if (g[vertex.position][vertexTree[vertex][0].position] is Resistor && 
                                !ElementsToDelete.ContainsKey(g[vertex.position][vertexTree[vertex][0].position]))
                                ElementsToDelete.Add(g[vertex.position][vertexTree[vertex][0].position], stepCount);
                        }
                    }
                    else
                    {
                        var verticesToRemove = new List<TreeElement>();
                        foreach (var vertexToCheck in vertexTree[vertex])
                        {
                            TreeElement lastVertexIndBranch = null;
                            Conductor conductor = null;
                            if (BranchContainsOnlyConductor(vertexToCheck, ref lastVertexIndBranch, ref conductor))
                            {
                                bool hasVertex = false;
                                foreach (var testVertex in verticesInIteration)
                                {
                                    if (testVertex.position.Equals(lastVertexIndBranch.position))
                                        hasVertex = true;
                                }

                                if (hasVertex)
                                {
                                    if (!ElementsToDelete.ContainsKey(conductor))
                                        ElementsToDelete.Add(conductor, stepCount);
                                    DeleteBranch(vertexToCheck);
                                    verticesToRemove.Add(vertexToCheck);
                                }

                                verticesInNextIteration.Add(vertex);
                            }
                        }

                        foreach (var vertexToRemove in verticesToRemove)
                        {
                            vertexTree[vertex].Remove(vertexToRemove);
                        }
                    }
                }

                stepCount++;
                verticesInIteration = new List<TreeElement>(verticesInNextIteration);
                verticesInNextIteration.Clear();
            }
        }

        private void DeleteElements(ElementBase element, int delay)
        {
            //Thread.Sleep(400 * delay);

            //Scheme.RemoveElement(element);
            //map.SetTile(element.pivotPosition, new Tile());
        }

        private bool BranchContainsOnlyConductor(TreeElement vertexToCheck, ref TreeElement lastVertexInBranch, ref Conductor conductor)
        {
            
            var elementsInBranch = new List<ElementBase>
            {
                g[vertexToCheck.root.position][vertexToCheck.position]
            };

            while (vertexTree[vertexToCheck].Count > 0)
            {
                if (vertexTree[vertexToCheck].Count > 1)
                    return false;

                vertexToCheck = vertexTree[vertexToCheck][0];
                elementsInBranch.Add(g[vertexToCheck.root.position][vertexToCheck.position]);
            }

            lastVertexInBranch = vertexToCheck;

            var conductorsCount = 0;
            foreach (var element in elementsInBranch)
            {
                switch (element)
                {
                    case Resistor _:
                        return false;
                    case Conductor _:
                        conductorsCount++;
                        conductor = (Conductor) element;
                        break;
                }
            }

            return conductorsCount <= 1;
        }

        private void DeleteBranch(TreeElement vertexToCheck)
        {
            List<TreeElement> treeElementsToDelete = new List<TreeElement>(){vertexToCheck};
            while (vertexTree[vertexToCheck].Count > 0)
            {
                treeElementsToDelete.Add(vertexTree[vertexToCheck][0]);
                vertexToCheck = vertexTree[vertexToCheck][0];
            }

            foreach (var elementToDelete in treeElementsToDelete)
            {
                vertexTree.Remove(elementToDelete);
            }
        }

        private void MakeVertexTree()
        {
            vertexTree.Clear();
            nullorVertices.Clear();
            var list = new List<Vector2Int>(g.Keys);
            var nullor = Scheme.GetNullorElementsList();
            var nullatorRoot = new TreeElement((Vector2Int) Scheme.GetNullator().pivotPosition, null);
            var noratorRoot = new TreeElement((Vector2Int) Scheme.GetNorator().pivotPosition, null);
            for (var i = 0; i < 4; i++)
            {
                vertexTree.Add(
                    nullor[i / 2] is Nullator
                        ? new TreeElement(list[i], nullatorRoot)
                        : new TreeElement(list[i], noratorRoot), new List<TreeElement>());
                nullorVertices.Add(list[i]);
            }

            var verticesInIteration = new List<TreeElement>(vertexTree.Keys);
            var verticesInNextIteration = new List<TreeElement>();

            while (verticesInIteration.Count > 0)
            {
                foreach (var rootVertex in verticesInIteration)
                {
                    foreach (var connectedVertex in g[rootVertex.position].Keys)
                    {
                        if (!nullorVertices.Contains(connectedVertex) && !rootVertex.root.position.Equals(connectedVertex))
                        {
                            var newElement = new TreeElement(connectedVertex, rootVertex);
                            if (vertexTree.ContainsKey(newElement) && vertexTree[newElement].Contains(rootVertex))
                                continue;

                            vertexTree[rootVertex].Add(newElement);
                            vertexTree.Add(newElement, new List<TreeElement>());

                            var elementsWithSamePosition = vertexTree.Keys.Where(element => element.position.Equals(newElement.position) && element != newElement).ToList();
                            elementsWithSamePosition.Add(newElement);

                            if (!HasOneRoot(elementsWithSamePosition))
                            {
                                verticesInNextIteration.Add(newElement);
                            }
                        }
                    }
                }

                verticesInIteration = new List<TreeElement>(verticesInNextIteration);
                verticesInNextIteration.Clear();
            }
        }

        private static bool HasOneRoot(List<TreeElement> elements)
        {
            var elementsWay = new List<TreeElement>();
            var element = elements[elements.Count - 1];
            elements.Remove(element);

            while (element.root != null)
            {
                elementsWay.Add(element.root);
                element = element.root;
            }

            foreach (var elem in elements)
            {
                var temp = elem;
                while (temp.root != null)
                {
                    if (elementsWay.Contains(temp.root))
                        return true;
                    temp = temp.root;
                }
            }

            return false;
        }
    }
}
