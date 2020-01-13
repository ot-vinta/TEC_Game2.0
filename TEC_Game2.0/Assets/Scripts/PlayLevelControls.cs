using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayLevelControls : MonoBehaviour
{
    public void BackPressed()
    {
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }

    public void ImportPressed()
    {
        //TO DO
    }

    public void NullatorPressed()
    {
        GameObject map = GameObject.Find("Map");
        map.AddComponent<MouseListener>();
        map.GetComponent<MouseListener>().Init("Nullator");
    }

    public void NoratorPressed()
    {
        //TO DO
    }

    public void WirePressed()
    {
        //TO DO
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

    public void PlayPressed()
    {
        //TO DO
    }

    public void RestartPressed()
    {
        //TO DO
    }

    public void StatisticsPressed()
    {
        //TO DO
    }
}
