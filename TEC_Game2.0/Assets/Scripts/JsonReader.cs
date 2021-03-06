﻿using Assets.Models;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    class JsonReader
    {
        private static GameObject mapObject;
        private static Tilemap map;

        public JsonReader()
        {
            mapObject = GameObject.Find("Map");
            map = mapObject.GetComponent<Tilemap>();
        }

        public void ConvertToObject(string path)
        {
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string json = reader.ReadToEnd();
                reader.Close();

                ClearTilemap();
                Scheme.Clear();

                Elements elements = JsonUtility.FromJson<Elements>(json);
                PlaceElements(elements.Resistors, "Sprites/ResistorSprite");
                PlaceElements(elements.Conductors, "Sprites/ConductorSprite");
                PlaceElements(elements.Wires, "Sprites/HalfWireSprite");
                PlaceElements(elements.Nullators, "Sprites/NullatorSprite");
                PlaceElements(elements.Norators, "Sprites/NoratorSprite");
            }
            else
            {
                //Warning
            }
        }

        private void PlaceElements<T>(List<T> elements, string texturePath)
        {
            foreach (T element in elements)
            {
                PlaceElement(element as ElementBase, texturePath);
            }

        }

        private void PlaceElement(ElementBase element, string texturePath)
        {
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

            if (element is LabeledChainElement)
            {
                //GameObject.Find("Labels").Add //(((LabeledChainElement)element).label);
                Debug.Log("JsonReader: LabeledChainElement encountered");

                //((LabeledChainElement)element).label.transform.parent = GameObject.Find("Labels").transform;
                ((LabeledChainElement)element).AddLabel(((LabeledChainElement)element).labelStr, element.pivotPosition);
                if (element is Resistor)
                {
                    ((Resistor)element).FixLabel();
                }
                if (element is Conductor)
                {
                    ((Conductor)element).FixLabel();
                }

            }

            float scale = 1;
            Quaternion rotation = Quaternion.Euler(0, 0, element.angle);

            if (element is Wire)
            {
                Wire temp = (Wire)element;

                scale = temp.pivotPosition.x == temp.secondPosition.x
                    ? (Math.Abs(temp.pivotPosition.y - temp.secondPosition.y) + 0.01f) / 2
                    : (Math.Abs(temp.pivotPosition.x - temp.secondPosition.x) + 0.01f) / 2;
            }

            var m = tile.transform;

            m.SetTRS(Vector3.zero, rotation, new Vector3(scale, 1, 1));
            tile.transform = m;

            switch (texturePath)
            {
                case "Sprites/ResistorSprite":
                    tile.name = "Resistor";
                    break;
                case "Sprites/ConductorSprite":
                    tile.name = "Conductor";
                    break;
                case "Sprites/HalfWireSprite":
                    tile.name = "Wire";
                    break;
                case "Sprites/NullatorSprite":
                    tile.name = "Nullator";
                    break;
                case "Sprites/NoratorSprite":
                    tile.name = "Norator";
                    break;
            }

            map.SetTile(element.pivotPosition, tile);
        }

        private void ClearTilemap()
        {
            foreach (var element in Scheme.elements.Values)
            {
                if (element is LabeledChainElement labeledElem)
                {
                    GameObject.Destroy(labeledElem.label);
                }
                map.SetTile(element.pivotPosition, null);
            }
        }
    }
}
