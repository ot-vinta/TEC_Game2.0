using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class JsonReader
    {
        public static void ConvertToObject(string path)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            string line = json.Substring(16, json.IndexOf('\n') - 19);

            while (json != "")
            {
                AddElementToScheme(line);

                if (json.IndexOf('\n') == -1)
                    json = json.Substring(json.IndexOf(']') + 2);
                else
                    json = json.Substring(json.IndexOf('\n') + 1);

                if (json.IndexOf('\n') == -1 && json != "")
                    line = json.Substring(2, json.IndexOf(']') - 3);
                else if (json != "")
                    line = json.Substring(2, json.IndexOf('\n') - 5);
            }

        }

        private static void AddElementToScheme(string line)
        {
            string type = line.Substring(0, line.IndexOf('\"'));
            line = line.Substring(line.IndexOf('\"') + 3);
            Debug.Log(line);

            switch (type)
            {
                case "Resistor":
                    PlaceElement(JsonUtility.FromJson<Resistor>(line), "Sprites/ResistorSprite");
                    break;
                case "Conductor":
                    PlaceElement(JsonUtility.FromJson<Conductor>(line), "Sprites/ConductorSprite");
                    break;
                case "Nullator":
                    PlaceElement(JsonUtility.FromJson<Nullator>(line), "Sprites/NullatorSprite");
                    break;
                case "Norator":
                    PlaceElement(JsonUtility.FromJson<Norator>(line), "Sprites/NoratorSprite");
                    break;
                case "Wire":
                    PlaceElement(JsonUtility.FromJson<Wire>(line), "Sprites/HalfWireSprite");
                    break;
            }
        }

        private static void PlaceElement(ElementBase element, string texturePath)
        {
            GameObject mapObject = GameObject.Find("Map");
            Tilemap map = mapObject.GetComponent<Tilemap>();

            Scheme.AddElement(element);
            Texture2D texture = Resources.Load<Texture2D>(texturePath);

            Tile tile;
            if (element is Wire)
                tile = new Tile
                {
                    sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.0f, 0.5f),
                        100,
                        1,
                        SpriteMeshType.Tight,
                        Vector4.zero
                    )
                };
            else
                tile = new Tile
                {
                    sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100,
                        1,
                        SpriteMeshType.Tight,
                        Vector4.zero
                    )
                };

            float scale = 1;
            Quaternion rotation = Quaternion.Euler(0, 0, element.angle);

            if (element is Wire)
            {
                Wire temp = (Wire) element;

                scale = temp.pivotPosition.x == temp.secondPosition.x
                    ? (Math.Abs(temp.pivotPosition.y - temp.secondPosition.y) + 0.01f) / 2
                    : (Math.Abs(temp.pivotPosition.x - temp.secondPosition.x) + 0.01f) / 2;
            }

            var m = tile.transform;

            m.SetTRS(Vector3.zero, rotation, new Vector3(scale, 1, 1));
            tile.transform = m;

            map.SetTile(element.pivotPosition, tile);
        }
    }
}
