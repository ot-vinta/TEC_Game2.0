using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class TilePlacer : MonoBehaviour
{
    private Tilemap map;
    private GameObject mapObject;
    private Tile elementTile;
    private Sprite elementPlacement;
    private SpriteRenderer sr;
    private GameObject empty;
    private Texture2D texture;
    private bool leftMousePressed;
    private bool horizontalPlaced;
    private float angle;
    private bool isInfinite;

    private bool wirePlacing;
    private int wireEndsCount;
    private Vector3 wireFirstPosition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(empty);
            mapObject.GetComponent<TileEditor>().BackupElement();
            GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
            mapObject.GetComponent<TilePlacer>().enabled = false;
        }

        if (!wirePlacing)
        {
            MoveSprite();
            RotateSprite();
        }
        else 
            PlaceWire();
    }

    public void Init(string type, int startAngle, bool isInfinite, string label = null)
    {
        this.isInfinite = isInfinite;
        mapObject = GameObject.Find("Map");
        map = mapObject.GetComponent<Tilemap>();
        elementTile = null;
        horizontalPlaced = true;
        leftMousePressed = false;
        wirePlacing = false;
        angle = startAngle;

        empty = new GameObject();
        empty.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        sr = empty.AddComponent<SpriteRenderer>();
        sr.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        sr.sortingOrder = 1;

        MoveToNearestCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        string path = "";

        switch (type)
        {
            case "Nullator":
                path = "Sprites/NullatorSprite";
                InitTile(path);
                break;
            case "Norator":
                path = "Sprites/NoratorSprite";
                InitTile(path);
                break;
            case "Resistor":
                path = "Sprites/ResistorSprite";
                InitTile(path);
                break;
            case "Conductor":
                path = "Sprites/ConductorSprite";
                InitTile(path);
                break;
            case "Wire":
                wirePlacing = true;
                wireEndsCount = 0;
                path = "Sprites/TileChosenSprite";
                InitWire(path, new Vector2(0.5f, 0.5f), 1.0f);
                break;
        }

        elementTile.name = type;
        sr.transform.Rotate(0.0f, 0.0f, angle, Space.World);
    }

    private void InitTile(string path)
    {
        texture = Resources.Load<Texture2D>(path);
        elementPlacement = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        elementTile = new Tile
        {
            sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), // section of texture to use
                new Vector2(0.5f, 0.5f),
                100, // pixels per unity tile grid unit
                1,
                SpriteMeshType.Tight,
                Vector4.zero
            )
        };
        sr.sprite = elementPlacement;
    }

    private void InitWire(string path, Vector2 pivot, float transparency)
    {
        texture = Resources.Load<Texture2D>(path);
        elementPlacement = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), pivot, 100.0f);
        elementTile = new Tile
        {
            sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), // section of texture to use
                pivot,
                100, // pixels per unity tile grid unit
                1,
                SpriteMeshType.Tight,
                Vector4.zero
            )
        };
        sr.sprite = elementPlacement;
        sr.color = new Color(1.0f, 1.0f, 1.0f, transparency);
    }

    private void MoveSprite()
    {
        if (!leftMousePressed)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);

            MoveToNearestCell(position);
        }
        
        if (Input.GetMouseButtonDown(0) && !leftMousePressed && !PressedUnderButton())
        {
            PlaceTile(Vector3.one, 1);
            Destroy(empty);

            //------------------------------------------------------
            //Add classes for chain elements
            //Change to specific element class(TO DO)
            Vector3Int pos = map.WorldToCell(sr.transform.position);
            AddElementToScheme(new LabeledChainElement(new Vector3Int(pos.x, pos.y, 1), (int) angle));
            //------------------------------------------------------

            if (isInfinite)
                Init(elementTile.name, 0, true);
            else
            {
                GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
                mapObject.GetComponent<TilePlacer>().enabled = false;
            }
        }
        else if (Input.GetMouseButtonDown(0) && PressedUnderButton())
        {
            Destroy(empty);
            GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
            mapObject.GetComponent<TilePlacer>().enabled = false;
        }
    }

    private void RotateSprite()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            horizontalPlaced = !horizontalPlaced;
            angle += (float) (90.0 % 360.0);
            sr.transform.Rotate(0.0f, 0.0f, 90.0f, Space.World);
        }
    }

    private void PlaceWire()
    {
        bool horsizontal = true;
        if (wireEndsCount == 0)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);

            MoveToNearestCell(position);

            if (Input.GetMouseButtonDown(0) && !PressedUnderButton())
            {
                Vector2 delta = new Vector2(0.25f, 0.25f);
                position = map.CellToWorld(map.WorldToCell(position));

                wireFirstPosition = new Vector3(position.x, position.y, Scheme.GetWiresCount() + 2);
                wireEndsCount = 1;

                string path = "Sprites/HalfWireSprite";
                InitWire(path, new Vector2(0.0f, 0.5f), 0.6f);
            }
            else if (Input.GetMouseButtonDown(0) && PressedUnderButton())
            {
                Destroy(empty);
                GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
                mapObject.GetComponent<TilePlacer>().enabled = false;
            }
        }
        else if (wireEndsCount == 1)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);
            float scale;

            Vector2 delta = new Vector2(0.25f, 0.25f);
            position = map.CellToWorld(map.WorldToCell(position));

            Vector3 wireSecondPosition = new Vector3(position.x, position.y, Scheme.GetWiresCount() + 2);
            if (Math.Abs(wireSecondPosition.x - wireFirstPosition.x) >
                Math.Abs(wireSecondPosition.y - wireFirstPosition.y))
            {
                angle = wireSecondPosition.x - wireFirstPosition.x > 0 ? 0.0f : 180.0f;
                horsizontal = true;
                scale = Math.Abs(wireSecondPosition.x - wireFirstPosition.x);
                RotateAndScaleWire(angle, scale);
            }
            else
            {
                angle = wireSecondPosition.y - wireFirstPosition.y > 0 ? 90.0f : 270.0f;
                horsizontal = false;
                scale = Math.Abs(wireSecondPosition.y - wireFirstPosition.y);
                RotateAndScaleWire(angle, scale);
            }

            if (Input.GetMouseButtonDown(0) && !PressedUnderButton())
            {
                wireEndsCount = 2;
                PlaceTile(new Vector3(scale, 1, 1), Scheme.GetWiresCount() + 2);
                Destroy(empty);

                Vector3Int pos1 = map.WorldToCell(wireFirstPosition);
                pos1 = new Vector3Int(pos1.x, pos1.y, Scheme.GetWiresCount() + 2);
                Vector3Int pos2 = map.WorldToCell(wireSecondPosition);
                pos2 = new Vector3Int(pos2.x, pos2.y, Scheme.GetWiresCount() + 2);

                if (horsizontal)
                    pos2.y = pos1.y;
                else
                    pos2.x = pos1.x;

                AddElementToScheme(new Wire(pos1, pos2));

                if (isInfinite)
                    Init("Wire", 0, true);
                else
                {
                    GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
                    mapObject.GetComponent<TilePlacer>().enabled = false;
                }
            }
            else if (Input.GetMouseButtonDown(0) && PressedUnderButton())
            {
                Destroy(empty);
                GameObject.Find("MainMenu").GetComponent<Map>().enabled = true;
                mapObject.GetComponent<TilePlacer>().enabled = false;
            }
        }
    }

    private void PlaceTile(Vector3 scale, int zPosition)
    {
        Vector3 position = map.WorldToCell(sr.transform.position);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        var m = elementTile.transform;

        m.SetTRS(Vector3.zero, rotation, scale);
        elementTile.transform = m;
        map.SetTile(new Vector3Int((int) position.x, (int) position.y, zPosition), elementTile);

        leftMousePressed = true;

        if (!isInfinite)
        {
            mapObject.GetComponent<TileEditor>().SetMove();
            mapObject.GetComponent<TileEditor>().DeleteBackup();
        }
    }

    private void RotateAndScaleWire(float angle, float scale)
    {
        angle -= sr.transform.eulerAngles.z;
        sr.transform.Rotate(0.0f, 0.0f, angle, Space.World);
        scale /= 2;
        empty.transform.localScale = new Vector3(scale, 0.5f, 1.0f);
    }

    private void MoveToNearestCell(Vector3 newPosition)
    {
        Vector2 delta = new Vector2(0.25f, 0.25f);
        Vector3 position = map.CellToWorld(map.WorldToCell(newPosition));

        sr.transform.position = new Vector3(position.x + delta.x, position.y + delta.x, 0);
    }

    private bool PressedUnderButton()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject  playButton = GameObject.Find("PlayButton");
        Vector3 cameraPos = Camera.main.transform.position;
        Vector2 LeftUpPos = new Vector2(-8.9f + cameraPos.x, 5.0f + cameraPos.y);
        Vector2 LeftDownPos = new Vector2(-8.9f + cameraPos.x, -5.0f + cameraPos.y);
        Vector2 RightUpPos = new Vector2(8.9f + cameraPos.x, 5.0f + cameraPos.y);
        Vector2 RightDownPos = new Vector2(8.9f + cameraPos.x, -5.0f + cameraPos.y);
        Vector2 elementsRightUpPos = new Vector2(-1.5f + cameraPos.x, -3.7f + cameraPos.y);
        Vector2 exportRightDownPos = new Vector2(-6.1f + cameraPos.x, 4.1f + cameraPos.y);
        if (playButton != null) exportRightDownPos.x = -7.0f + cameraPos.x;
        Vector2 editLeftUpPos = new Vector2(6.1f + cameraPos.x, -4.1f + cameraPos.y);
        Vector2 PlayLeftDownPos = new Vector2(8.0f + cameraPos.x, 4.1f + cameraPos.y);
        Vector2 StatsLeftDownPos = new Vector2(8.4f + cameraPos.x, 3.1f + cameraPos.y);

        bool ans = InRect(LeftUpPos, exportRightDownPos, mousePos) ||
                   InRect(LeftDownPos, elementsRightUpPos, mousePos) ||
                   InRect(RightDownPos, editLeftUpPos, mousePos);
        if (playButton != null)
            ans = ans || InRect(RightUpPos, PlayLeftDownPos, mousePos) ||
                  InRect(RightUpPos, StatsLeftDownPos, mousePos);

        return ans;
    }

    private bool InRect(Vector2 firstPos, Vector2 secondPos, Vector2 checkPos)
    {
        float x1 = firstPos.x < secondPos.x ? firstPos.x : secondPos.x;
        float x2 = firstPos.x > secondPos.x ? firstPos.x : secondPos.x;
        float y1 = firstPos.y < secondPos.y ? firstPos.y : secondPos.y;
        float y2 = firstPos.y > secondPos.y ? firstPos.y : secondPos.y;

        return (checkPos.x > x1 && checkPos.x < x2 && checkPos.y > y1 && checkPos.y < y2);
    }

    private void AddElementToScheme(ElementBase element)
    {
        Scheme.AddElement(element);
    }

    public void SetAngle(int angle)
    {
        this.angle = angle;
        if (sr != null)
            sr.transform.Rotate(0.0f, 0.0f, angle, Space.World);
    }
}
