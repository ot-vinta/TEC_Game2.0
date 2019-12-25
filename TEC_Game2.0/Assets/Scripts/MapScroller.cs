using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class MapScroller : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        GameObject map = GameObject.Find("Grid");
        Vector3 delta = Camera.main.ScreenToViewportPoint(eventData.delta);
        map.transform.position += new Vector3(delta.x*15, delta.y*15, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Start");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End");
    }
}
