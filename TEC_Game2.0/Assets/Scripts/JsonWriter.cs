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
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Levels"));
            string json = JsonUtility.ToJson(elements);
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(json);
            writer.Close();
        }
    }
}
