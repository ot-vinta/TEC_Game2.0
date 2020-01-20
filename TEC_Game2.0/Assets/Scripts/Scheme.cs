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
        private static ElementBase[,,] chainElements = new ElementBase[Map.MapSizeX, Map.MapSizeY, 1000];
        private static List<ElementBase> elements = new List<ElementBase>();
        private static int wiresCount = 0;

        public static void AddElement(ElementBase element)
        {
            chainElements[element.pivotPosition.x, element.pivotPosition.y, element.pivotPosition.z] = element;
            elements.Add(element);
            element.SetId(elements.Count);
            if (element is Wire) wiresCount++;
            if (wiresCount > 1000) wiresCount = 0;
        }

        public static void RemoveElement(ElementBase element)
        {
            chainElements[element.pivotPosition.x, element.pivotPosition.y, element.pivotPosition.z] = null;
            elements.Remove(element);
        }
        public static void RemoveElement(Vector3Int pos)
        {
            elements.Remove(chainElements[pos.x, pos.y, pos.z]);
            chainElements[pos.x, pos.y, pos.z] = null;
        }

        public static ElementBase GetElement(Vector3Int position)
        {
            if (position.x <= 60 && position.x >= 1 && position.y <= 45 && position.y >= 1)
                return chainElements[position.x, position.y, position.z];
            else
                return null;
        }

        public static ElementBase GetElement(int id)
        {
            return elements[id - 1];
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
            chainElements = new ElementBase[Map.MapSizeY, Map.MapSizeX, 1000];
            wiresCount = 0;
        }
    }
}
