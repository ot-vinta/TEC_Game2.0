using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class UIChooseBox
    {
        // Пример вызова диалога можно посмотреть в PlayLevelControls в RestartPressed()
        public GameObject dialogCanvas;
        public Button cancelButton;
        public Dictionary<string, GameObject> buttons;
        public ScrollRect scrollRect;
        private GameObject contentField;
        private readonly DirectoryInfo importDirectory;
        private readonly string importPath;

        //Predicate<string> OnClickAction;

        public UIChooseBox()
        {
            dialogCanvas = GameObject.Find("UI_ChooseBox");
            cancelButton = dialogCanvas.GetComponentInChildren<Button>();
            scrollRect = dialogCanvas.GetComponentInChildren<ScrollRect>();
            contentField = scrollRect.content.gameObject;

            importDirectory = new DirectoryInfo(Application.persistentDataPath + "/Levels/");
            importPath = Application.persistentDataPath + "/Levels/";
            buttons = new Dictionary<string, GameObject>();
            HideDialog();
        }
        
        private void ChooseLevelTask(string levelName)
        {
            JsonReader reader = new JsonReader();
            reader.ConvertToObject(importPath + levelName);
            HideDialog();
        }

        private void CancelTask()
        {
            HideDialog();
        }
        
        public void ShowDialog()
        {
            List<string> fileNames = GetFilesList();

            dialogCanvas.SetActive(true);

            for (int i = 0; i < fileNames.Count; i++)
                if (!buttons.ContainsKey(fileNames[i]))
                {
                    GameObject newButton = GameObject.Instantiate(Resources.Load("Prefabs/ButtonPrefab")) as GameObject;
                    newButton.transform.position = contentField.transform.position;
                    newButton.transform.position += new Vector3(0, -i * 0.55f, 0); 
                    newButton.GetComponent<RectTransform>().SetParent(contentField.transform);
                    newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    newButton.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 200);

                    newButton.GetComponentInChildren<Text>().text = fileNames[i];

                    newButton.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        ChooseLevelTask(newButton.GetComponentInChildren<Text>().text);
                    });
                    buttons.Add(fileNames[i], newButton);
                }

            scrollRect.content.sizeDelta = new Vector2(0, 25 * fileNames.Count);

            cancelButton.GetComponentInChildren<Text>().text = "Отмена";
            cancelButton.onClick.AddListener(CancelTask);
        }

        public void HideDialog()
        {
            dialogCanvas.SetActive(false);
        }

        public bool IsDialogActive() => dialogCanvas.activeSelf;

        private List<string> GetFilesList()
        {
            List<string> result = new List<string>();
            FileInfo[] files = importDirectory.GetFiles("*.json");

            foreach (var file in files)
            {
                result.Add(file.Name);
            }

            return result;
        }
    }
}
