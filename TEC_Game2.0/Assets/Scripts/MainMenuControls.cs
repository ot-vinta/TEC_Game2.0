using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{

    public void StartPressed()
    {
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
    }

    public void CreatePressed()
    {
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

