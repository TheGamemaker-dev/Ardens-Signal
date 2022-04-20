using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DoubleClickHandler : MonoBehaviour, IPointerDownHandler {
    public UnityAction doubleClicked;
    
    float lastClickTime = 0;
    
    static readonly float doubleClickDelay = 0.5f;

    public void OnPointerDown(PointerEventData eventData){
        if(lastClickTime == 0){
            lastClickTime = Time.time;
            return;
        }

        float clickDifference = Time.time - lastClickTime;

        if(clickDifference <= doubleClickDelay){
            doubleClicked?.Invoke();
        }

        lastClickTime = Time.time;
    }
}
