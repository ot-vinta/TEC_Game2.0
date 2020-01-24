using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class Map : MonoBehaviour, IDragHandler
{
    private const double MapXLeft = -6.2;
    private const double MapXRight = 6.2;
    private const double MapYUp = 6.1;
    private const double MapYDown = -6.1;
    public const double Scale = -0.0167;
    public const int MapSizeX = 60;
    public const int MapSizeY = 45;

    public void OnDrag(PointerEventData eventData)
    {
        GameObject camera = GameObject.Find("Main Camera");
        GameObject map = GameObject.Find("Canvas");
        
        Vector3 delta = eventData.delta;
        Vector3 position = map.transform.position;

        if (position.x + delta.x * Scale > MapXLeft &&
            position.x + delta.x * Scale < MapXRight)
        {
            map.transform.position += new Vector3((float) (delta.x * Scale), 0, 0);
            camera.transform.position += new Vector3((float)(delta.x * Scale), 0, 0);
        }

        if (position.y + delta.y * Scale < MapYUp &&
            position.y + delta.y * Scale > MapYDown)
        {
            map.transform.position += new Vector3(0, (float) (delta.y * Scale), 0);
            camera.transform.position += new Vector3(0, (float)(delta.y * Scale), 0);
        }
    }
}
