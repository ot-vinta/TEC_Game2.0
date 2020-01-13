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
    private Tile elementTile;
    private Sprite elementPlacement;
    private SpriteRenderer sr;
    private Texture2D texture;
    private bool leftMousePressed = false;

    void Awake()
    {
        GameObject empty = new GameObject();
        empty.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        sr = empty.AddComponent<SpriteRenderer>();
        sr.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        transform.position = new Vector3(1.5f, 1.5f, 0.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<Tilemap>();
        elementTile = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftMousePressed)
        {
            Vector3 delta = Input.mouseScrollDelta;
            sr.transform.position += delta;
        }

        if (Input.GetMouseButtonDown(0))
        {
            leftMousePressed = true;
        }
    }

    public void Init(string type)
    {
        switch (type)
        {
            case "Nullator":
                texture = Resources.Load<Texture2D>("Sprites/NullatorSprite");
                elementPlacement = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                
                sr.sprite = elementPlacement;
                break;
        }
    }
}
