using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MouseListener : MonoBehaviour
{
    private Tilemap map;
    private GameObject mapObject;
    private Tile elementTile;
    private Sprite elementPlacement;
    private SpriteRenderer sr;
    private GameObject empty;
    private Texture2D texture;
    private bool leftMousePressed = false;
    private Vector3 mousePrevPosition;

    // Start is called before the first frame update
    void Start()
    {
        mapObject = GameObject.Find("Map");
        map = mapObject.GetComponent<Tilemap>();
        elementTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftMousePressed)
        {
            Vector3 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePrevPosition = Input.mousePosition;
            Vector3 position = sr.transform.position + new Vector3((float) (delta.x * Map.Scale), (float) (delta.y * Map.Scale), 0);

            sr.transform.position = new Vector3(delta.x, delta.y, 0);
        }
        else
        {
            Destroy(empty);
            mapObject.GetComponent<MouseListener>().enabled = false;
            leftMousePressed = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftMousePressed = true;
        }
    }

    public void Init(string type)
    {
        empty = new GameObject();
        empty.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        sr = empty.AddComponent<SpriteRenderer>();
        sr.color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        sr.sortingOrder = 1;
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        sr.transform.position = new Vector3(position.x, position.y, 0);

        switch (type)
        {
            case "Nullator":
                texture = Resources.Load<Texture2D>("Sprites/NullatorSprite");
                elementPlacement = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                
                sr.sprite = elementPlacement;
                mousePrevPosition = Input.mousePosition;
                break;
        }
    }
}
