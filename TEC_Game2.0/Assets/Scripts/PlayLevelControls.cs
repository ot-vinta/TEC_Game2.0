using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayLevelControls : MonoBehaviour
{

    UIInputBox dialog;

    private void Start()
    {
        dialog = new UIInputBox();
    }

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
        map.GetComponent<TilePlacer>().enabled = true;
        map.GetComponent<TilePlacer>().Init("Nullator");
    }

    public void NoratorPressed()
    {
        GameObject map = GameObject.Find("Map");
        map.GetComponent<TilePlacer>().enabled = true;
        map.GetComponent<TilePlacer>().Init("Norator");
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

    public void PlayPressed()
    {
        dialog.SetOnClickListener(message =>
        {
            dialog.title.text = message;
            return true;
        });
        dialog.ShowDialog("Проверка вызова из PlayLevelControls");
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
