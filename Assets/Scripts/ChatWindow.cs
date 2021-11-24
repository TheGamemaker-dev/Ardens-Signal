using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatWindow : MonoBehaviour, IPointerDownHandler
{
    public bool wasLastClickedOn = false;

    bool wasClickedOn = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!wasClickedOn && Input.GetMouseButtonDown(0))
        {
            wasLastClickedOn = false;
        }
        wasClickedOn = false;
    }
    public void OnPointerDown(PointerEventData data)
    {
        wasLastClickedOn = true;
        wasClickedOn = true;
    }
}
