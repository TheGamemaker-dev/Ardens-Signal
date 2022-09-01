using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BrowserLink : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    BrowserPage linkTo;

    BrowserPage parentPage;

    private void Start()
    {
        parentPage = GetComponentInParent<BrowserPage>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log("Click!");
        parentPage.ChangePage(linkTo);
    }
}
