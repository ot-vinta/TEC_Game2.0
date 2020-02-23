using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayLevelControls : MonoBehaviour
{
    private GameObject map;
    UIInputBox dialog;
    UIList dialogList;
    private UIChooseBox chooseDialog;
    void Start()
    {
        map = GameObject.Find("Map");
        dialog = new UIInputBox();
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
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Nullator", 0, true);
    }

    public void NoratorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Norator", 0, true);
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

    public void PlayPressed() //Надо будет передвинуть
    {
        var connectionGraph = ConnectionsMaker.MakeConnectionGraph();
        if (connectionGraph.Count == 0)
        {
            string[] alarmShit = new string[1];
            alarmShit[0] = "Схема не связана";
            dialogList.ShowDialog(alarmShit);
        }
        else
        {
            SchemeSimplifier simplifier = new SchemeSimplifier(connectionGraph);
            var elementsToDelete = simplifier.SimplifyAsync();

            foreach (var element in elementsToDelete.Keys)
            {
                map.GetComponent<Tilemap>().SetTile(element.pivotPosition, new Tile());
                Scheme.RemoveElement(element);
            }
        }
    }

    public void RestartPressed() //Надо будет передвинуть
    {
        string[] elements = new string[15];
        elements[0] = "Проводимость: 4";
        elements[1] = "Сопротивление: 2";
        elements[2] = "Провод: 3";
        elements[3] = "Если";
        elements[4] = "Строк";
        
        elements[5] = "Слишком";
        elements[6] = "Много";
        elements[7] = "То";
        elements[8] = "Должен";
        elements[9] = "Появиться";
        elements[10] = "Скролл";
        elements[11] = "Бар";
        elements[12] = "Вот";
        elements[13] = "Как";
        elements[14] = "Сейчас";
        dialogList.ShowDialog(elements);
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
}
