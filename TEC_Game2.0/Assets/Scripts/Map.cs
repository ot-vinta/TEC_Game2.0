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
    private const double MapXLeft = -9.2;
    private const double MapXRight = -21.8;
    private const double MapYUp = -18.1;
    private const double MapYDown = -5.2;
    public const double Scale = 0.0167;
    public const int MapSizeX = 60;
    public const int MapSizeY = 45;

    public void OnDrag(PointerEventData eventData)
    {
        GameObject map = GameObject.Find("Map");
        Vector3 delta = eventData.delta;
        Vector3 position = map.transform.position;

        if (position.x + delta.x * Scale > MapXRight &&
            position.x + delta.x * Scale < MapXLeft)
            map.transform.position += new Vector3((float)(delta.x * Scale), 0, 0);

        if (position.y + delta.y * Scale < MapYDown &&
            position.y + delta.y * Scale > MapYUp)
            map.transform.position += new Vector3(0, (float)(delta.y * Scale), 0);
    }
}
