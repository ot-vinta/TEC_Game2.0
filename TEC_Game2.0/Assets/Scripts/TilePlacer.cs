﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
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

    private bool wirePlacing;
    private int wireEndsCount;
    private Vector3 wireFirstPosition;
    private int wireZPosition = 1;


    // Update is called once per frame
    void Update()
    {
        if (!wirePlacing)
        {
            MoveSprite();
            RotateSprite();
        }
        else 
            PlaceWire();
    }

    public void Init(string type)
    {
        mapObject = GameObject.Find("Map");
        map = mapObject.GetComponent<Tilemap>();
        elementTile = null;
        horizontalPlaced = true;
        leftMousePressed = false;
        wirePlacing = false;
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
                wirePlacing = true;
                wireEndsCount = 0;
                path = "Sprites/TileChosenSprite";
                InitWire(path, new Vector2(0.5f, 0.5f), 1.0f);
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
        
        if (Input.GetMouseButtonDown(0) && !leftMousePressed)
        {
            PlaceTile(Vector3.one, 1);
            Destroy(empty);
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
        if (wireEndsCount == 0)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);

            MoveToNearestCell(position);

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 delta = new Vector2(0.25f, 0.25f);
                position = map.CellToWorld(map.WorldToCell(position));

                wireFirstPosition = new Vector3(position.x + delta.x, position.y + delta.x, 1);
                Debug.Log(wireFirstPosition);
                wireEndsCount = 1;

                string path = "Sprites/HalfWireSprite";
                InitWire(path, new Vector2(0.0f, 0.5f), 0.6f);
            }
        }
        else if (wireEndsCount == 1)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = new Vector3(newPos.x, newPos.y, 0);
            float scale;

            Vector2 delta = new Vector2(0.25f, 0.25f);
            position = map.CellToWorld(map.WorldToCell(position));

            Vector3 wireSecondPosition = new Vector3(position.x + delta.x, position.y + delta.x, 1);
            Debug.Log(wireSecondPosition);
            if (Math.Abs(wireSecondPosition.x - wireFirstPosition.x) >
                Math.Abs(wireSecondPosition.y - wireFirstPosition.y))
            {
                angle = wireSecondPosition.x - wireFirstPosition.x > 0 ? 0.0f : 180.0f;
                scale = Math.Abs(wireSecondPosition.x - wireFirstPosition.x);
                RotateAndScaleWire(angle, scale);
            }
            else
            {
                angle = wireSecondPosition.y - wireFirstPosition.y > 0 ? 90.0f : 270.0f;
                scale = Math.Abs(wireSecondPosition.y - wireFirstPosition.y);
                RotateAndScaleWire(angle, scale);
            }

            if (Input.GetMouseButtonDown(0))
            {
                wireEndsCount = 2;
                PlaceTile(new Vector3(scale, 1, 1), wireZPosition);
                wireZPosition++;
                Destroy(empty);
                mapObject.GetComponent<TilePlacer>().enabled = false;
            }
        }
    }

    private void PlaceTile(Vector3 scale, int zPosition)
    {
        Vector3 position = map.WorldToCell(sr.transform.position);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        var m = elementTile.transform;

        //Here we can add element to list later
        m.SetTRS(Vector3.zero, rotation, scale);
        elementTile.transform = m;
        map.SetTile(new Vector3Int((int) position.x, (int) position.y, zPosition), elementTile);

        leftMousePressed = true;
    }

    private void RotateAndScaleWire(float angle, float scale)
    {
        angle -= sr.transform.eulerAngles.z;
        Debug.Log(sr.transform.rotation.z);
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

    public void SetLeftMousePressed()
    {
        leftMousePressed = true;
    }
}