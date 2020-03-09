using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class ElementGraph
    {
        Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>> graph = new Dictionary<Vector2Int, Dictionary<Vector2Int, ElementBase>>();
        private void AddElement(Vector2Int end1, Vector2Int end2, ElementBase elem)
        {

            Vector2Int toCompare = new Vector2Int(-1,-1), //Vector2Int не может быть null. Приходится выкручиваться
                       equalToEnd1 = toCompare,
                       equalToEnd2 = toCompare;
            
            foreach (KeyValuePair<Vector2Int, Dictionary<Vector2Int, ElementBase>> keyValuePair in graph)
            {
                if (keyValuePair.Key.Equals(end1))
                {
                    equalToEnd1 = keyValuePair.Key;
                }
                if (keyValuePair.Key.Equals(end2))
                {
                    equalToEnd2 = keyValuePair.Key;
                }
            }
            //Добавляем элемент в graph[end1]
            if (equalToEnd1 == toCompare)
            {
                Dictionary<Vector2Int, ElementBase> innerGraph = new Dictionary<Vector2Int, ElementBase>
                {
                    { end2, elem }
                };
                graph.Add(end1, innerGraph);
            }
            else
            {
                Vector2Int innerEqualToEnd = toCompare;
                foreach (KeyValuePair<Vector2Int, ElementBase> keyValuePair in graph[equalToEnd1])
                {
                    if (keyValuePair.Key.Equals(end2))
                    {
                        innerEqualToEnd = keyValuePair.Key;
                    }
                }
                if (innerEqualToEnd == toCompare)
                {
                    graph[equalToEnd1].Add(end2, elem);
                } 
                else
                {
                    graph[equalToEnd1][innerEqualToEnd] = elem;
                }
            }
            //Добавляем элемент в graph[end2]
            if (equalToEnd2 == toCompare)
            {
                Dictionary<Vector2Int, ElementBase> innerGraph = new Dictionary<Vector2Int, ElementBase>
                {
                    { end1, elem }
                };
                graph.Add(end2, innerGraph);
            }
            else
            {
                Vector2Int innerEqualToEnd = toCompare;
                foreach (KeyValuePair<Vector2Int, ElementBase> keyValuePair in graph[equalToEnd2])
                {
                    if (keyValuePair.Key.Equals(end1))
                    {
                        innerEqualToEnd = keyValuePair.Key;
                    }
                }
                if (innerEqualToEnd == toCompare)
                {
                    graph[equalToEnd2].Add(end1, elem);
                }
                else
                {
                    graph[equalToEnd2][innerEqualToEnd] = elem;
                    Debug.Log("Элемент с такими координатами уже есть в графе, он был заменён");
                }
            }
        }
        public ElementBase GetElement(Vector2Int end1, Vector2Int end2)
        {
            Vector2Int toCompare = new Vector2Int(-1, -1), //Vector2Int не может быть null. Приходится выкручиваться
                      equalToEnd1 = toCompare;

            foreach (KeyValuePair<Vector2Int, Dictionary<Vector2Int, ElementBase>> keyValuePair in graph)
            {
                if (keyValuePair.Key.Equals(end1))
                {
                    equalToEnd1 = keyValuePair.Key;
                }
            }
            if (equalToEnd1 == toCompare)
            {
                Debug.Log("Такого элемента в графе нет");
                return null;
            }
            else
            {
                Vector2Int innerEqualToEnd = toCompare;
                foreach (KeyValuePair<Vector2Int, Dictionary<Vector2Int, ElementBase>> keyValuePair in graph)
                {
                    if (keyValuePair.Key.Equals(end1))
                    {
                        innerEqualToEnd = keyValuePair.Key;
                    }
                }
                if (innerEqualToEnd == toCompare)
                {
                    Debug.Log("Такого элемента в графе нет");
                    return null;
                }
                else
                {
                    return graph[equalToEnd1][innerEqualToEnd];
                }
            }
        }

        private void RemoveElement(Vector2Int end1, Vector2Int end2)
        {
            Vector2Int toCompare = new Vector2Int(-1, -1), //Vector2Int не может быть null. Приходится выкручиваться
                       equalToEnd1 = toCompare,
                       equalToEnd2 = toCompare;
            foreach (KeyValuePair<Vector2Int, Dictionary<Vector2Int, ElementBase>> keyValuePair in graph)
            {
                if (keyValuePair.Key.Equals(end1))
                {
                    equalToEnd1 = keyValuePair.Key;
                }
                if (keyValuePair.Key.Equals(end2))
                {
                    equalToEnd2 = keyValuePair.Key;
                }
            }
            if (equalToEnd1 == toCompare)
            {
                Debug.Log("Такого элемента в графе и так нет");
            }
            else
            {
                Vector2Int innerEqualToEnd = toCompare;
                foreach (KeyValuePair<Vector2Int, ElementBase> keyValuePair in graph[equalToEnd1])
                {
                    if (keyValuePair.Key.Equals(end2))
                    {
                        innerEqualToEnd = keyValuePair.Key;
                    }
                }
                if (innerEqualToEnd == toCompare)
                {
                    Debug.Log("Такого элемента в графе и так нет");
                }
                else
                {
                    graph[equalToEnd1][innerEqualToEnd] = null;
                }
            }
            // Удаляем вторую ссылку на элемент
            if (equalToEnd2 == toCompare)
            {
                Debug.Log("Такого элемента в графе и так нет");
            }
            else
            {
                Vector2Int innerEqualToEnd = toCompare;
                foreach (KeyValuePair<Vector2Int, ElementBase> keyValuePair in graph[equalToEnd2])
                {
                    if (keyValuePair.Key.Equals(end1))
                    {
                        innerEqualToEnd = keyValuePair.Key;
                    }
                }
                if (innerEqualToEnd == toCompare)
                {
                    Debug.Log("Такого элемента в графе и так нет");
                }
                else
                {
                    graph[equalToEnd2][innerEqualToEnd] = null;
                }
            }
        }
        public void AddElement(ElementBase elem)
        {
            Vector2Int end1 = ConnectionsMaker.GetConnectPosition(true, elem),
                       end2 = ConnectionsMaker.GetConnectPosition(false, elem);
            AddElement(end1, end2, elem);
        }
        public void RemoveElement(ElementBase elem)
        {
            Vector2Int end1 = ConnectionsMaker.GetConnectPosition(true, elem),
                       end2 = ConnectionsMaker.GetConnectPosition(false, elem);
            RemoveElement(end1, end2);
        }
    }
}
