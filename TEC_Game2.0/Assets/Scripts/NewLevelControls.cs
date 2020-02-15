using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevelControls : MonoBehaviour
{
    private GameObject map;
    private UIInputBox dialog;
    private UIChooseBox chooseDialog;
    private string exportPath;
    private string importPath;

    void Start()
    {
        map = GameObject.Find("Map");
        dialog = new UIInputBox();
        chooseDialog = new UIChooseBox();

        exportPath = importPath = Application.persistentDataPath + "/Levels/";
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public void SavePressed()
    {
        dialog.SetOnClickListener(message =>
        {
            if (message != "")
            {
                exportPath += message + ".json";
                JsonWriter.ConvertToJson(Scheme.ToSerializableElements(), exportPath);
                exportPath = Application.persistentDataPath + "/Levels/";
                dialog.HideDialog();
            }
            else
                dialog.title.text = "Введите какое-нибудь имя!";
            return true;
        });

        dialog.ShowDialog("Название схемы:", "Сохранить");
    }

    public void ImportPressed()
    {
        chooseDialog.ShowDialog();
    }

    public void ConductorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Conductor", 0, true);
    }

    public void ResistorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Resistor", 0, true);
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
}
