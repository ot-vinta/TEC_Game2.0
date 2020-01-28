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
        UI_InputBox dial = Resources.FindObjectsOfTypeAll<UI_InputBox>()[0]; //Понимаю, выглядит устрашающе, но это единственный способ (из того, что я нашёл), 
                                                                             //позволяющий получить неактивный объект нужного класса
        dial.ShowDialogue("Проверка вызова из PlayLevelControls");
        string r = dial.GetReply(); //Если попытаться получить ответ сразу после вывода диалога, получишь null (Или то, что пользователь вводил в прошлый раз)
        //Всё будет хорошо, если пользователь сам нажмёт на какую-нибудь другую кнопку после ввода строки в диалог
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
