using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Models;
using UnityEngine;

namespace Assets.Scripts
{
    class JsonWriter
    {

        public static void ConvertToJson(Elements elements, string path)
        {
            string json = JsonUtility.ToJson(elements);
            File.WriteAllText(path, json);
        }
    }
}
