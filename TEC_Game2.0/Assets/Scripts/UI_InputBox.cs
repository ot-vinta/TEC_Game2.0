using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInputBox
{
    // Пример вызова диалога можно посмотреть в PlayLevelControls в PlayPressed()
    public GameObject dialogCanvas;
    public Button button;
    public InputField inputField;
    public Text title;

    Predicate<string> OnClickAction;

    public UIInputBox()
    {
        dialogCanvas = GameObject.Find("UI_InputBox");
        button = dialogCanvas.GetComponentInChildren<Button>();
        inputField = dialogCanvas.GetComponentInChildren<InputField>();
        title = dialogCanvas.GetComponentInChildren<Text>();
        HideDialog();
    }

    public void SetOnClickListener(Predicate<string> onClickAction)
    {
        button.onClick.AddListener(delegate { onClickAction(inputField.text); });
    }

    public void ShowDialog(string title, string buttonText = "Ok")
    {
        dialogCanvas.SetActive(true);
        this.title.text = title;
        inputField.text = "";
        button.GetComponentInChildren<Text>().text = buttonText;
    }

    public void HideDialog()
    {
        dialogCanvas.SetActive(false);
    }

    public bool IsDialogActive() => dialogCanvas.activeSelf;

}
