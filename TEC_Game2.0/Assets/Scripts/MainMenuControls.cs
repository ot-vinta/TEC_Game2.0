using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{	
	UIList dialogList;
	
	void Start()
    {
        dialogList = new UIList();
    }

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
        var alarmShit = new string[16];
		alarmShit[0] = "Программа TEC 2.0";
		alarmShit[1] = "";
		alarmShit[2] = "Целью программы является возможность";
		alarmShit[3] = "продемонстрировать процесс упрощения схем с";
		alarmShit[4] = "использованием нуллатора/норатора";
		alarmShit[5] = "";
		alarmShit[6] = "Авторы:";
		alarmShit[7] = "Поздняков Михаил Владимирович";
		alarmShit[8] = "Калашников Иван Геннадьевич";
		alarmShit[9] = "Апанасович Кирилл Сергеевич";
		alarmShit[10] = "Аксенов Владимир Алексеевич";
		alarmShit[11] = "";
		alarmShit[12] = "Руководитель";
		alarmShit[13] = "Горшков Константин Сергеевич";
		alarmShit[14] = "";
		alarmShit[15] = "ИТМО 2020";
		dialogList.ShowDialog(alarmShit);
    }
}

