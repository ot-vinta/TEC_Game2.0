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

    public UIInputBox(Vector2 position)
    {
        TileEditor te = GameObject.FindObjectOfType<TileEditor>();
        te.SetDefault();
        TilePlacer tp = GameObject.FindObjectOfType<TilePlacer>();
        tp.CancelPlacing();

        dialogCanvas = GameObject.Instantiate(Resources.Load("Prefabs/InputBoxPrefab")) as GameObject;
        dialogCanvas.transform.position = new Vector3(position.x, position.y, 0);
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
