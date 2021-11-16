using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowTopBar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    const float screenToUIRatio = 5f / 12f;
    RectTransform rectTransform;
    Vector2 currentPos;
    Vector2 differenceVector;
    Vector2 mouseInUISpace;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.parent.transform as RectTransform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentPos = rectTransform.localPosition;
        mouseInUISpace = ((Vector2)Input.mousePosition) * screenToUIRatio;
        differenceVector = currentPos - mouseInUISpace;

    }

    public void OnDrag(PointerEventData eventData)
    {
        mouseInUISpace = ((Vector2)Input.mousePosition) * screenToUIRatio;
        rectTransform.localPosition = mouseInUISpace + differenceVector;
    }
}
