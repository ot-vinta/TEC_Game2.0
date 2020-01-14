using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewLevelControls : MonoBehaviour
{
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
        GameObject map = GameObject.Find("Map");
        map.GetComponent<TilePlacer>().enabled = true;
        map.GetComponent<TilePlacer>().Init("Conductor");
    }

    public void ResistorPressed()
    {
        GameObject map = GameObject.Find("Map");
        map.GetComponent<TilePlacer>().enabled = true;
        map.GetComponent<TilePlacer>().Init("Resistor");
    }

    public void WirePressed()
    {
        GameObject map = GameObject.Find("Map");
        map.GetComponent<TilePlacer>().enabled = true;
        map.GetComponent<TilePlacer>().Init("Wire");
    }

    public void DeletePressed()
    {
        //TO DO
    }

    public void RotatePressed()
    {
        //TO DO
    }

    public void MovePressed()
    {
        //TO DO
    }
}
