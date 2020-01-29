using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayLevelControls : MonoBehaviour
{

    UIInputBox dialog;
    UIList dialogList;

    private void Start()
    {
        dialog = new UIInputBox();
        dialogList = new UIList();
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
        //TO DO
    }
}
