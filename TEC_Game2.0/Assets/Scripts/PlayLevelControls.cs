﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts;
using Assets.Scripts.SchemeSimplifying;
using Assets.Scripts.utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayLevelControls : MonoBehaviour
{
    private GameObject map;
    UIInputBox dialog;
    UIList dialogList;
    private UIChooseBox chooseDialog;
    private BackupController _backupController;
    private Dictionary<string, List<string>> deletedElements;

    void Start()
    {
        map = GameObject.Find("Map");
        dialog = new UIInputBox(new Vector2(350, 350));
        dialogList = new UIList();
        _backupController = BackupController.GetInstance();

        chooseDialog = new UIChooseBox();

        deletedElements = new Dictionary<string, List<string>>();
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public void ImportPressed()
    {
        if (chooseDialog.IsDialogActive())
            chooseDialog.HideDialog();
        else
            chooseDialog.ShowDialog();
    }

    public void NullatorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();

        if (Scheme.GetNullator() != null)
        {
            Alarm("Нуллатор уже есть!");
        }
        else
        {
            map.GetComponent<TilePlacer>().enabled = true;
            GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
            map.GetComponent<TilePlacer>().Init("Nullator", 0, true);
        }
    }

    public void NoratorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();

        if (Scheme.GetNorator() != null)
        {
            Alarm("Норатор уже есть!");
        }
        else { 
            map.GetComponent<TilePlacer>().enabled = true;
            GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
            map.GetComponent<TilePlacer>().Init("Norator", 0, true);
        }
    }

    public void WirePressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Wire", 0, true);
    }

    public void DeletePressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() == TileEditor.StatusDelete)
            map.GetComponent<TileEditor>().SetDelete();
        else
            map.GetComponent<TileEditor>().SetDelete();
    }

    public void RotatePressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() == TileEditor.StatusRotate)
            map.GetComponent<TileEditor>().SetRotate();
        else
            map.GetComponent<TileEditor>().SetRotate();
    }

    public void MovePressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() == TileEditor.StatusMove)
            map.GetComponent<TileEditor>().SetMove();
        else
            map.GetComponent<TileEditor>().SetMove();
    }

    public void PlayPressed()
    {
        var connectionGraph = ConnectionsMaker.MakeConnectionGraph();
        if (connectionGraph.Count == 0 || Scheme.GetNorator() == null || Scheme.GetNullator() == null)
        {
            var alarmShit = new string[3];
            alarmShit[0] = "Схема не связана";
            alarmShit[1] = "или не стоит";
            alarmShit[2] = "нуллатор с норатором!";
            dialogList.ShowDialog(alarmShit);
        }
        else
        {
            _backupController.Backup();

            var simplifier = new SchemeSimplifier(connectionGraph);
            var elementsToDelete = simplifier.Simplify();

            StartCoroutine(DeleteElements(elementsToDelete));
        }
    }

    public void RestartPressed()
    {
        var isBackuped = _backupController.Restart();
    }

    private IEnumerator DeleteElements(Dictionary<int, List<ElementBase>> elementsToDelete)
    {
        foreach (var time in elementsToDelete.Keys)
        {
            foreach (var element in elementsToDelete[time])
            {
                if (element is LabeledChainElement chainElement)
                {
                    string elemType = chainElement.ToString();
                    if (deletedElements.ContainsKey(elemType))
                    {
                        deletedElements[elemType].Add(chainElement.labelStr);
                    }
                    else
                    {
                        List<string> list = new List<string>
                        {
                            chainElement.labelStr
                        };
                        deletedElements.Add(elemType, list);
                    }
                    var label = chainElement.label;
                    Destroy(label);
                }

                if (element is Resistor)
                {
                    ReplaceWithWire(element);
                }
                Scheme.RemoveElement(element);
                map.GetComponent<Tilemap>().SetTile(element.pivotPosition, new Tile());
            }

            yield return new WaitForSeconds(0.6f);
        }
    }

    public void StatisticsPressed()
    {
        List<string> arrayToShow = new List<string>();
        
        foreach (KeyValuePair<string,List<string>> elemType in deletedElements)
        {
            arrayToShow.Add(elemType.Key + " (" + elemType.Value.Count + ")" + ":");
            foreach(string elementName in elemType.Value)
            {
                arrayToShow.Add("– " + elementName);
            }
        }
        dialogList.ShowDialog(arrayToShow.ToArray());
    }
    
    private void Alarm(string text)
    {
        var elements = new string[1];
        elements[0] = text;
        dialogList.ShowDialog(elements);
    }

    private void ReplaceWithWire(ElementBase element)
    {
        var firstPos = new Vector3Int(ConnectionsMaker.GetConnectPosition(true, element).x, ConnectionsMaker.GetConnectPosition(true, element).y, Scheme.GetWiresCount() + 2);
        var secondPos = new Vector3Int(ConnectionsMaker.GetConnectPosition(false, element).x, ConnectionsMaker.GetConnectPosition(false, element).y, Scheme.GetWiresCount() + 2);
        var wire = new Wire(firstPos, secondPos, element.angle);

        Scheme.AddElement(wire);
        var texture = Resources.Load<Texture2D>("Sprites/HalfWireSprite");

        var tile = new Tile
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

        var scale = wire.pivotPosition.x == wire.secondPosition.x
            ? (Math.Abs(wire.pivotPosition.y - wire.secondPosition.y) + 0.01f) / 2
            : (Math.Abs(wire.pivotPosition.x - wire.secondPosition.x) + 0.01f) / 2;
        var rotation = Quaternion.Euler(0, 0, element.angle);

        var m = tile.transform;

        m.SetTRS(Vector3.zero, rotation, new Vector3(scale, 1, 1));
        tile.transform = m;

        tile.name = "Wire";

        map.GetComponent<Tilemap>().SetTile(wire.pivotPosition, tile);
    }
}
