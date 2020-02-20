using System.Collections;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileEditor : MonoBehaviour
{
    private GameObject mapObject;
    private Tilemap map;
    private Tile prevTile, selectedTile;
    private Vector3Int prevPos;
    private string editStatus;
    private ElementBase backupElement;
    private Vector3Int backupPos;
    private Tile backupTile;

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

            if (selectedTile != null && selectedTile != prevTile)
            {
                MarkTile(selectedTile, mousePos);
                UnmarkTile(prevTile, prevPos);
            }
            else if (selectedTile == null)
                UnmarkTile(prevTile, prevPos);

            if (selectedTile != null && Scheme.GetElement(mousePos) is Wire && (editStatus == StatusMove || editStatus == StatusRotate))
            {
                UnmarkTile(selectedTile, mousePos);
            }

            prevPos = mousePos;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && editStatus != StatusDefault)
        {
            SetDefault();
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
                    if (!(Scheme.GetElement(mousePos) is Wire))
                    {
                        Debug.Log(Scheme.GetElement(mousePos));
                        RotateElement(mousePos, selectedTile);
                    }

                    break;
                case StatusMove:
                    if (!(Scheme.GetElement(mousePos) is Wire))
                        MoveElement(mousePos, selectedTile);
                    break;
            }

            selectedTile = null;
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

        List<Wire> wires = Scheme.GetWiresList();
        foreach (var wire in wires)
        {
            if ((wire.pivotPosition.x == pos.x && wire.secondPosition.x == pos.x &&
                 wire.pivotPosition.y <= pos.y && wire.secondPosition.y >= pos.y) ||
                (wire.pivotPosition.y == pos.y && wire.secondPosition.y == pos.y &&
                 wire.pivotPosition.x <= pos.x && wire.secondPosition.x >= pos.x) ||
                (wire.pivotPosition.x == pos.x && wire.secondPosition.x == pos.x &&
                 wire.pivotPosition.y >= pos.y && wire.secondPosition.y <= pos.y) ||
                (wire.pivotPosition.y == pos.y && wire.secondPosition.y == pos.y &&
                 wire.pivotPosition.x >= pos.x && wire.secondPosition.x <= pos.x))
            {
                selectedPos = wire.pivotPosition;
                Debug.Log(pos);
                Debug.Log(wire.pivotPosition);
                return map.GetTile<Tile>(wire.pivotPosition);
            }
        }
        return null;
    }

    private void MarkTile(Tile tile, Vector3Int pos)
    {
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
        ElementBase elem = Scheme.GetElement(pos);
        if (elem is LabeledChainElement)
        {
            Text label = ((LabeledChainElement)elem).label;
            Destroy(label);
        }
        Scheme.RemoveElement(pos);
        map.SetTile(pos, new Tile());
    }

    private void RotateElement(Vector3Int pos, Tile tile)
    {
        int angle = Scheme.GetRotation(pos);
        angle += 90 % 360;
        Scheme.RotateElement(pos, angle);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        map.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one));
    }

    private void MoveElement(Vector3Int pos, Tile tile)
    {
        backupElement = Scheme.GetElement(pos);
        string label = null;
        if (backupElement is LabeledChainElement)
        {
            Text labelElem = ((LabeledChainElement)backupElement).label;
            label = labelElem.text;
            Destroy(labelElem);
            //LabeledChainElement added = (LabeledChainElement) Scheme.GetLatestElement();
            //added.SetName(label);
        }
        if (GetStatus() != StatusDefault)
            SetDefault();
        mapObject.GetComponent<TilePlacer>().enabled = true;
        mapObject.GetComponent<TilePlacer>().Init(tile.name, Scheme.GetRotation(pos), false, label);

        
        backupPos = pos;
        backupTile = tile;

        DeleteElement(pos);
    }

    public void BackupElement()
    {
        if (backupElement != null)
        {
            Scheme.AddElement(backupElement);
            map.SetTile(backupPos, backupTile);
            SetMove();
        }
    }

    public void DeleteBackup()
    {
        backupElement = null;
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
