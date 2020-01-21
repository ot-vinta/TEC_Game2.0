using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevelControls : MonoBehaviour
{
    private GameObject map;

    void Start()
    {
        map = GameObject.Find("Map");
    }

    public void BackPressed()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public void SavePressed()
    {
        //TO DO
    }

    public void ImportPressed()
    {
        //TO DO
    }

    public void ConductorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Conductor", 0);
    }

    public void ResistorPressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Resistor", 0);
    }

    public void WirePressed()
    {
        if (map.GetComponent<TileEditor>().GetStatus() != TileEditor.StatusDefault)
            map.GetComponent<TileEditor>().SetDefault();
        map.GetComponent<TilePlacer>().enabled = true;
        GameObject.Find("MainMenu").GetComponent<Map>().enabled = false;
        map.GetComponent<TilePlacer>().Init("Wire", 0);
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
