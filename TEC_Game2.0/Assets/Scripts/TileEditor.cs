using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    private GameObject mapObject;
    private Tilemap map;
    private Tile prevTile, selectedTile;
    private Vector3Int prevPos;
    private string editStatus;

    public const string StatusDelete = "delete";
    public const string StatusRotate = "rotate";
    public const string StatusMove = "move";
    public const string StatusDefault = "default";

    void Start()
    {
        mapObject = GameObject.Find("Map");
        map = mapObject.GetComponent<Tilemap>();
        editStatus = StatusDefault;
        selectedTile = null;
    }

    void Update()
    {
        Vector3Int mousePos = map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (editStatus != StatusDefault)
        {
            prevTile = selectedTile;
            selectedTile = FindTile(ref mousePos);
            //Debug.Log(selectedTile);

            if (selectedTile != null && selectedTile != prevTile)
            {
                MarkTile(selectedTile, mousePos);
                UnmarkTile(prevTile, prevPos);
            }
            else if (selectedTile == null)
                UnmarkTile(prevTile, prevPos);

            prevPos = mousePos;
        }

        if (prevTile != null && editStatus == StatusDefault)
        {
            UnmarkTile(prevTile, prevPos);
            prevTile = null;
        }

        if (Input.GetMouseButtonDown(0) && selectedTile != null)
        {
            switch (editStatus)
            {
                case StatusDelete:
                    DeleteElement(mousePos);
                    break;
                case StatusRotate:
                    RotateElement(mousePos);
                    break;
                case StatusMove:
                    MoveElement(mousePos);
                    break;
            }

            selectedTile = null;
            SetDefault();
        }
    }

    private Tile FindTile(ref Vector3Int position)
    {
        Vector3Int selectedPos = new Vector3Int(position.x, position.y, 1);
        if ((Scheme.GetElement(selectedPos) != null) && !(Scheme.GetElement(selectedPos) is Wire))
        {
            position = selectedPos;
            return map.GetTile<Tile>(selectedPos);
        }

        selectedPos.x++;
        if ((Scheme.GetElement(selectedPos) != null) && !(Scheme.GetElement(selectedPos) is Wire))
        {
            position = selectedPos;
            return map.GetTile<Tile>(selectedPos);
        }

        selectedPos.x -= 2;
        if ((Scheme.GetElement(selectedPos) != null) && !(Scheme.GetElement(selectedPos) is Wire))
        {
            position = selectedPos;
            return map.GetTile<Tile>(selectedPos);
        }

        selectedPos.x++;
        selectedPos.y++;
        if ((Scheme.GetElement(selectedPos) != null) && !(Scheme.GetElement(selectedPos) is Wire))
        {
            position = selectedPos;
            return map.GetTile<Tile>(selectedPos);
        }

        selectedPos.y -= 2;
        if ((Scheme.GetElement(selectedPos) != null) && !(Scheme.GetElement(selectedPos) is Wire))
        {
            position = selectedPos;
            return map.GetTile<Tile>(selectedPos);
        }

        selectedPos.y++;
        position = selectedPos;
        return FindWire(ref position);
    }

    private Tile FindWire(ref Vector3Int selectedPos)
    {
        Vector3Int pos = selectedPos;
        for (int i = 1; i <= Scheme.GetElementsCount(); i++)
        {
            Debug.Log(Scheme.GetElement(i));
            Wire wire = null;
            if (Scheme.GetElement(i) is Wire)
                wire = (Wire) Scheme.GetElement(i);
            else continue;

            if ((wire.pivotPosition.x == pos.x && wire.secondPosition.x == pos.x &&
                 wire.pivotPosition.y <= pos.y && wire.secondPosition.y >= pos.y) ||
                (wire.pivotPosition.y == pos.y && wire.secondPosition.y == pos.y &&
                 wire.pivotPosition.x <= pos.x && wire.secondPosition.x >= pos.x))
            {
                selectedPos = wire.pivotPosition;
                return map.GetTile<Tile>(wire.pivotPosition);
            }
        }

        return null;
    }

    private void MarkTile(Tile tile, Vector3Int pos)
    {
        Debug.Log("Changing color");
        if (tile != null)
        {
            map.SetTileFlags(new Vector3Int(pos.x, pos.y, pos.z), TileFlags.None);
            map.SetColor(new Vector3Int(pos.x, pos.y, pos.z), new Color(1.0f, 1.0f, 1.0f, 0.45f));
        }
    }

    private void UnmarkTile(Tile tile, Vector3Int pos)
    {
        if (tile != null)
        {
            map.SetTileFlags(new Vector3Int(pos.x, pos.y, pos.z), TileFlags.None);
            map.SetColor(new Vector3Int(pos.x, pos.y, pos.z), new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
    }

    private void DeleteElement(Vector3Int pos)
    {
        Scheme.RemoveElement();
        map.SetTile(pos, new Tile());
    }

    private void RotateElement(Vector3Int pos)
    {

    }

    private void MoveElement(Vector3Int pos)
    {

    }

    public void SetDelete()
    {
        //string path = "";
        //ChangeCursor(path);

        editStatus = StatusDelete;
    }

    public void SetRotate()
    {
        //string path = "";
        //ChangeCursor(path);

        editStatus = StatusRotate;
    }

    public void SetMove()
    {
        //string path = "";
        //ChangeCursor(path);

        editStatus = StatusMove;
    }

    public void SetDefault()
    {
        //string path = "";
        //ChangeCursor(path);

        editStatus = StatusDefault;
    }

    public string GetStatus()
    {
        return editStatus;
    }

    private void ChangeCursor(string path)
    {
        //Maybe it's useless, but TO DO
    }
}
