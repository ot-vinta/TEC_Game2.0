using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputBox : MonoBehaviour
{
    // Пример вызова диалога можно посмотреть в PlayLevelControls в PlayPressed()


    GameObject dialogue_canvas;
    Button button; //Пока не используется.
    InputField inputField;
    bool OKpressed;
    string reply;

    private void Awake()
    {
        this.dialogue_canvas = GameObject.Find("UI_InputBox");
        this.button = dialogue_canvas.GetComponentInChildren<Button>();
        this.inputField = dialogue_canvas.GetComponentInChildren<InputField>();
        this.OKpressed = false;
        this.HideDialogue();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    
    void Update()
    {
        if (this.OKpressed)
        {
            this.reply = this.inputField.text;
            this.HideDialogue();
            this.inputField.text = null;
            this.OKpressed = false;
        }
    }
    public void ShowDialogue(string message)
    {
        this.dialogue_canvas.GetComponentInChildren<Text>().text = message;
        this.dialogue_canvas.SetActive(true);
    }
    public void HideDialogue()
    {
        this.dialogue_canvas.SetActive(false);
    }
    public void OnClickOk()
    {
        this.OKpressed = true;
    }
    public string GetReply()
    {
        return this.reply;
    }

    //Ниже находятся методы, которые нигде не используются, но могут быть полезны в будущем

    public bool CheckIfActive()
    {
        return this.dialogue_canvas.activeSelf;
    }
    public void ShowDialogue()
    {
        this.dialogue_canvas.SetActive(true);
    }

    //Ниже находятся мои попытки использовать корутины. Если удалите это к чертям собачьим, я против не буду.

    /*
    public string GetReply()
    {
        StartCoroutine(ReplyCoroutine("Shit"));
        
        string reply = this.inputField.text;
        this.OKpressed = false;
        this.inputField.text = null;
        return reply;
    }
    public void GetReply(string message)
    {
        ShowDialogue(message);
        StartCoroutine(ReplyCoroutine(message));
        Debug.Log("GetReply: " + "Coroutine is finished");
    }

    public IEnumerator ReplyCoroutine(string message)
    {
        Debug.Log("ReplyCoroutine: " + "Dialogue is shown");
        yield return new WaitUntil(() => this.OKpressed);
        Debug.Log("ReplyCoroutine: " + "yield returned");
        //yield return new WaitForSeconds(100);
        string reply = this.inputField.text;
        this.reply = reply;
        this.OKpressed = false;
        Debug.Log("ReplyCoroutine: " + "OKpressed = false");
        this.HideDialogue();
        Debug.Log("ReplyCoroutine: " + "Dialogue is hidden");
        this.inputField.text = null;
    }
    public IEnumerator ReplyCoroutine()
    {
        Debug.Log("ReplyCoroutine: " + "Dialogue is shown");
        yield return new WaitUntil(() => this.OKpressed);
        Debug.Log("ReplyCoroutine: " + "yield returned");
        //yield return new WaitForSeconds(100);
        string reply = this.inputField.text;
        this.reply = reply;
        this.OKpressed = false;
        Debug.Log("ReplyCoroutine: " + "OKpressed = false");
        this.HideDialogue();
        Debug.Log("ReplyCoroutine: " + "Dialogue is hidden");
        this.inputField.text = null;

    }*/
}