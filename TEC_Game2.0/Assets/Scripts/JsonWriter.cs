using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class JsonWriter
    {

        public static void ConvertToJson(Dictionary<int, ElementBase> elements, List<Wire> wires, string path)
        {
            string json = "[";

            List<ElementBase> elem = new List<ElementBase>(elements.Values);

            for (int i = 0; i < elem.Count; i++)
            {
                if (i != elem.Count - 1)
                    json = json + "{\"" + elem[i] + "\": " + JsonUtility.ToJson(elem[i]) + "}, \n";
                else
                    json = json + "{\"" + elem[i] + "\": " + JsonUtility.ToJson(elem[i]) + "}]";
            }

            File.WriteAllText(path, json);
        }
    }
}
