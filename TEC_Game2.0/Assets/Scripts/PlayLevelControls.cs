using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts;
using Assets.Scripts.SchemeSimplifying;
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
    void Start()
    {
        map = GameObject.Find("Map");
        dialog = new UIInputBox(new Vector2(350, 350));
        dialogList = new UIList();

        chooseDialog = new UIChooseBox();
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
            string[] alarmShit = new string[3];
            alarmShit[0] = "Схема не связана";
            alarmShit[1] = "или не стоит";
            alarmShit[2] = "нуллатор с норатором!";
            dialogList.ShowDialog(alarmShit);
        }
        else
        {
            SchemeSimplifier simplifier = new SchemeSimplifier(connectionGraph);
            var elementsToDelete = simplifier.Simplify();

            foreach (var element in elementsToDelete.SelectMany(elementsInOnTiming => elementsInOnTiming.Value))
            {
                if (element is LabeledChainElement chainElement)
                {
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
        }
    }

    public void RestartPressed() 
    {
        
    }

    public void StatisticsPressed()
    {
        dialog.SetOnClickListener(message =>
        {
            dialog.title.text = message;
            return true;
        });
        dialog.ShowDialog("Проверка вызова из PlayLevelControls");
    }

    private void Alarm(String text)
    {
        string[] elements = new string[1];
        elements[0] = text;
        dialogList.ShowDialog(elements);
    }

    private void ReplaceWithWire(ElementBase element)
    {
        Vector3Int firstPos = new Vector3Int(ConnectionsMaker.GetConnectPosition(true, element).x, ConnectionsMaker.GetConnectPosition(true, element).y, Scheme.GetWiresCount() + 2);
        Vector3Int secondPos = new Vector3Int(ConnectionsMaker.GetConnectPosition(false, element).x, ConnectionsMaker.GetConnectPosition(false, element).y, Scheme.GetWiresCount() + 2);
        Wire wire = new Wire(firstPos, secondPos, element.angle);

        Scheme.AddElement(wire);
        Texture2D texture = Resources.Load<Texture2D>("Sprites/HalfWireSprite");

        Tile tile = new Tile
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

        float scale = wire.pivotPosition.x == wire.secondPosition.x
            ? (Math.Abs(wire.pivotPosition.y - wire.secondPosition.y) + 0.01f) / 2
            : (Math.Abs(wire.pivotPosition.x - wire.secondPosition.x) + 0.01f) / 2;
        Quaternion rotation = Quaternion.Euler(0, 0, element.angle);

        var m = tile.transform;

        m.SetTRS(Vector3.zero, rotation, new Vector3(scale, 1, 1));
        tile.transform = m;

        tile.name = "Wire";

        map.GetComponent<Tilemap>().SetTile(wire.pivotPosition, tile);
    }
}
