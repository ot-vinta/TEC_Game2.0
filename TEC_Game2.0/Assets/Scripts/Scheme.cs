using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    static class Scheme
    {
        private static ElementBase[,,] chainElements = new ElementBase[Map.MapSizeX + 1, Map.MapSizeY + 1, 10000];
        private static Dictionary<int, ElementBase> elements = new Dictionary<int, ElementBase>();
        private static int wiresCount = 0;

        public static void AddElement(ElementBase element)
        {
            chainElements[element.pivotPosition.x, element.pivotPosition.y, element.pivotPosition.z] = element;
            elements.Add(elements.Count + 1, element);
            element.SetId(elements.Count);
            if (element is Wire) wiresCount++;
        }

        public static void RemoveElement(ElementBase element)
        {
            chainElements[element.pivotPosition.x, element.pivotPosition.y, element.pivotPosition.z] = null;
            elements.Remove(element.GetId());
        }
        public static void RemoveElement(Vector3Int pos)
        {
            elements.Remove(chainElements[pos.x, pos.y, pos.z].GetId());
            chainElements[pos.x, pos.y, pos.z] = null;
        }

        public static ElementBase GetElement(Vector3Int position)
        {
            if (position.x <= 60 && position.x >= 1 && position.y <= 45 && position.y >= 1)
                return chainElements[position.x, position.y, position.z];
            else
                return null;
        }

        public static int GetRotation(Vector3Int pos)
        {
            ChainElement elem = (ChainElement) chainElements[pos.x, pos.y, pos.z];
            return elem.angle;
        }

        public static void RotateElement(Vector3Int pos, int angle)
        {
            ChainElement elem = (ChainElement)chainElements[pos.x, pos.y, pos.z];
            elem.angle = angle;
        }

        public static List<Wire> GetWiresList()
        {
            List<Wire> ans = new List<Wire>();

            foreach (var element in elements)
            {
                if (element.Value is Wire)
                    ans.Add((Wire) element.Value);
            }

            return ans;
        }

        public static int GetWiresCount()
        {
            return wiresCount;
        }

        public static int GetElementsCount()
        {
            return elements.Count;
        }

        public static void Clear()
        {
            chainElements = new ElementBase[Map.MapSizeY, Map.MapSizeX, 10000];
            elements.Clear();
            wiresCount = 0;
        }
    }
}
