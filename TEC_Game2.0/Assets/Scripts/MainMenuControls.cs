using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{

    public void StartPressed()
    {
        Scheme.Clear();
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
    }

    public void CreatePressed()
    {
        Scheme.Clear();
        SceneManager.LoadScene("NewLevelScene", LoadSceneMode.Single);
    }

    public void ClosePressed()
    {
        Application.Quit();
    }

    public void HelpPressed()
    {
        //TO DO
    }
}

