using UnityEngine;
using UnityEngine.EventSystems;

public class WindowTopBar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    Vector2 screenToUIRatio = Vector2.zero;
    RectTransform rectTransform;
    Vector2 currentPos;
    Vector2 differenceVector;
    Vector2 mouseInUISpace;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.parent.transform as RectTransform;
        Vector2 canvasDimensions = (FindObjectOfType<Canvas>().transform as RectTransform).rect.size;
        screenToUIRatio = new Vector2(canvasDimensions.x / Screen.width, canvasDimensions.y / Screen.height);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentPos = rectTransform.localPosition;
        mouseInUISpace = Vector2.Scale(((Vector2)Input.mousePosition), screenToUIRatio);
        differenceVector = currentPos - mouseInUISpace;

    }

    public void OnDrag(PointerEventData eventData)
    {
        mouseInUISpace = Vector2.Scale(((Vector2)Input.mousePosition), screenToUIRatio);
        rectTransform.localPosition = mouseInUISpace + differenceVector;
    }

    public void CloseWindow()
    {
        GameObject parent = transform.parent.gameObject;
        parent.transform.SetAsFirstSibling();
    }
}
