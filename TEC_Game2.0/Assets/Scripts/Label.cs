using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        //Debug.Log("Наведён курсор на подпись");
        if (Input.GetMouseButtonDown(1))
        {
            // For debug
            Debug.Log("Нажата правая кнопка мыши на подпись");
            this.gameObject.GetComponent<Text>().text = "Изменено";
            /*
             * UIInputBox dialog = new UIInputBox();
            dialog.SetOnClickListener(message =>
            {
                dialog.title.text = message;
                return true;
            });
            dialog.ShowDialog("Проверка вызова из PlayLevelControls");
            */
        }
    }
}
