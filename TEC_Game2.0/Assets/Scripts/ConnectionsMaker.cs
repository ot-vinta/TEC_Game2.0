using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts
{
    class ConnectionsMaker
    {
        public static Dictionary<TreeElement, List<TreeElement>> MakeConnectionTree()
        {
            List<ElementBase> unusedElements = new List<ElementBase>(Scheme.elements.Values);
            List<ElementBase> elementsInTree = new List<ElementBase>();
            Dictionary<TreeElement, List<TreeElement>> connectionTree = new Dictionary<TreeElement, List<TreeElement>>();

            foreach (var element in Scheme.GetNullorElementsList())
            {
                Vector2Int leftPos = GetConnectPosition(true, element);
                Vector2Int rightPos = GetConnectPosition(false, element);

                connectionTree.Add(new TreeElement(element, leftPos), new List<TreeElement>());
                connectionTree.Add(new TreeElement(element, rightPos), new List<TreeElement>());

                unusedElements.Remove(element);
                elementsInTree.Add(element);
            }

            while (unusedElements.Count > 0)
            {
                List<ElementBase> removeElements = new List<ElementBase>();
                foreach (var element in unusedElements)
                {
                    Vector2Int leftPos, rightPos;
                    if (element is Wire)
                    {
                        Wire wire = (Wire) element;
                        leftPos = new Vector2Int(wire.pivotPosition.x, wire.pivotPosition.y);
                        rightPos = new Vector2Int(wire.secondPosition.x, wire.secondPosition.y);
                    }
                    else
                    {
                        leftPos = GetConnectPosition(true, element);
                        rightPos = GetConnectPosition(false, element);
                    }

                    Dictionary < TreeElement, List < TreeElement >> treeCopy = new Dictionary<TreeElement, List<TreeElement>>(connectionTree);

                    foreach (var treeElement in treeCopy.Keys)
                    {
                        if (treeElement.connectPosition.Equals(leftPos))
                        {
                            connectionTree[treeElement].Add(new TreeElement(element, leftPos));
                            connectionTree.Add(new TreeElement(element, rightPos), new List<TreeElement>());
                            removeElements.Add(element);
                        }
                        else if (treeElement.connectPosition.Equals(rightPos))
                        {
                            connectionTree[treeElement].Add(new TreeElement(element, rightPos));
                            connectionTree.Add(new TreeElement(element, leftPos), new List<TreeElement>());
                            removeElements.Add(element);
                        }
                    }
                }

                foreach (var element in removeElements)
                {
                    unusedElements.Remove(element);
                    elementsInTree.Add(element);
                }

                if (unusedElements.Count != 0 && removeElements.Count == 0)
                {
                    return new Dictionary<TreeElement, List<TreeElement>>();
                }
            }

            return connectionTree;
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
