﻿using Assets.Scripts;
using Assets.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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

            var mapObject = GameObject.Find("Map");
            Vector2 position = mapObject.GetComponent<Tilemap>().CellToWorld(element.pivotPosition);
            position = Camera.main.WorldToScreenPoint(position);

            UIInputBox dialog = new UIInputBox(position);
			var title = "Введите название элемента\n(до 4-ёх символов)";

            dialog.SetOnClickListener(message =>
            {
				if (message.Length > 4)
					dialog.title.text += "\nСлишком много символов!";
				else{
					element.SetName(message);
					dialog.HideDialog();
					GameObject.Destroy(dialog.dialogCanvas);
				}
				return true;
            });
            dialog.ShowDialog(title);
        

            //element.SetName(Random.Range(0,999).ToString());
        }
    }
}
