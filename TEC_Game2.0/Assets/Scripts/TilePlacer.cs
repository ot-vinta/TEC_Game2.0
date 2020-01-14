using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    // Update is called once per frame
    void Update()
    {
        MoveSprite();
        RotateSprite();
        PlaceTile();
    }

    public void Init(string type)
    {
        mapObject = GameObject.Find("Map");
        map = mapObject.GetComponent<Tilemap>();
        elementTile = null;
        horizontalPlaced = true;
        angle = 0.0f;

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
                break;
        }
    }

    private void InitTile(string path)
    {
        texture = Resources.Load<Texture2D>(path);
        elementPlacement = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        elementTile = new Tile
        {
            sprite = Sprite.Create(texture,
                new Rect(0, 0, 392, 140), // section of texture to use
                new Vector2(0.5f, 0.5f), // pivot in centre
                100, // pixels per unity tile grid unit
                1,
                SpriteMeshType.Tight,
                Vector4.zero
            )
        };
        sr.sprite = elementPlacement;
    }

    private void MoveToNearestCell(Vector3 newPosition)
    {
        Vector2 delta = new Vector2(0.25f, 0.25f);
        Vector3 position = map.CellToWorld(map.WorldToCell(newPosition));

        sr.transform.position = new Vector3(position.x + delta.x, position.y + delta.x, 0);
    }

    private void MoveSprite()
    {
        if (!leftMousePressed)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);

            MoveToNearestCell(position);
        }
        else
        {
            Destroy(empty);
            mapObject.GetComponent<TilePlacer>().enabled = false;
            leftMousePressed = false;
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

    private void PlaceTile()
    {
        if (Input.GetMouseButtonDown(0) && !leftMousePressed)
        {
            Vector3 position = map.WorldToCell(sr.transform.position);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            var m = elementTile.transform;

            m.SetTRS(Vector3.zero, rotation, Vector3.one);
            elementTile.transform = m;
            map.SetTile(new Vector3Int((int) position.x, (int) position.y, 1), elementTile);

            leftMousePressed = true;
        }
    }
}
