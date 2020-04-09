using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { Destroy(gameObject.GetComponentInParent<Canvas>()); } );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
