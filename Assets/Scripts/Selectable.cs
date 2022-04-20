using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selectable : MonoBehaviour, IPointerDownHandler
{

    bool clickedOn = false;
    Image background;

    public UnityEvent clicked;

    [SerializeField] Color selectedColor, unselectedColor;

    // Start is called before the first frame update
    public virtual void Start()
    {
        background = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!clickedOn && Input.GetMouseButtonDown(0))
        {
            background.color = unselectedColor;
        }
        clickedOn = false;
    }
    public void OnPointerDown(PointerEventData data)
    {
        clickedOn = true;
        clicked?.Invoke();
        background.color = selectedColor;
    }
}
