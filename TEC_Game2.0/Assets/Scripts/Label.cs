using Assets.Scripts;
using Assets.Models;
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
        if (Input.GetMouseButtonDown(1))
        {
            NewLevelControls scene = FindObjectOfType<NewLevelControls>();
            UIInputBox dialog = scene.dialog;
            Text label_text = this.gameObject.GetComponent<Text>();
            LabeledChainElement element = null;
            foreach (KeyValuePair<int, ElementBase> elem in Scheme.elements)
            {
                if (elem.Value is LabeledChainElement)
                {
                    if (((LabeledChainElement)elem.Value).label.gameObject == this.gameObject)
                    {
                        element = (LabeledChainElement)elem.Value;
                    }
                }
            }
            /*
            dialog.SetOnClickListener(message =>
            {
                element.SetName(message);
                dialog.HideDialog();
                return true;
            });
            dialog.ShowDialog("Введите название элемента");
            */

            element.SetName(Random.Range(0,999).ToString());
        }
    }
}
