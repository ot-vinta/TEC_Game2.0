using System;
using UnityEngine;
using UnityEngine.UI;

public class UIList
{
    // Пример вызова диалога можно посмотреть в PlayLevelControls в RestartPressed()
    public GameObject dialogCanvas;
    public Button button;
    public string[] elementList;
    public ScrollRect scrollRect;
    public Text contentText;

    //Predicate<string> OnClickAction;

    public UIList()
    {
        dialogCanvas = GameObject.Find("UI_List");
        button = dialogCanvas.GetComponentInChildren<Button>();
        scrollRect = dialogCanvas.GetComponentInChildren<ScrollRect>();
        contentText = scrollRect.GetComponentInChildren<Text>();
        
        HideDialog();
    }
    /* Здесь такое вроде не нужно будет
    public void SetOnClickListener(Predicate<string> onClickAction)
    {
        button.onClick.AddListener(delegate { onClickAction(inputField.text); });
    }
    */
    public void ShowDialog(string[] elements, string buttonText = "Ok")
    {
        dialogCanvas.SetActive(true);
        scrollRect.content.sizeDelta = new Vector2(0, 16 * elements.Length);
        contentText.rectTransform.sizeDelta = new Vector2(160, 16 * elements.Length);
        contentText.text = "";
        foreach (string elem in elements)
        {
            contentText.text += elem + "\n";
        }
        button.GetComponentInChildren<Text>().text = buttonText;
    }

    public void HideDialog()
    {
        dialogCanvas.SetActive(false);
    }

    public bool IsDialogActive() => dialogCanvas.activeSelf;

}
