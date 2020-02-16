using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UIInputBox dialog = new UIInputBox();
            //Text 
            Debug.Log("Нажата правая кнопка мыши на подпись");
            dialog.SetOnClickListener(message =>
            {
                dialog.title.text = message;
                return true;
            });
            dialog.ShowDialog("Проверка вызова из PlayLevelControls");
        }
    }
}
