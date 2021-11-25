using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatSelectable : MonoBehaviour, IPointerDownHandler
{

    bool clickedOn = false;
    PeopleList peopleList;
    Image background;

    public Text status;
    // Start is called before the first frame update
    void Start()
    {
        peopleList = GetComponentInParent<PeopleList>();
        background = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!clickedOn && Input.GetMouseButtonDown(0))
        {
            background.color = new Color(0, 0, 0, 0);
        }
        clickedOn = false;
    }
    public void OnPointerDown(PointerEventData data)
    {
        clickedOn = true;
        peopleList.ShowConversation(gameObject.name);
        background.color = new Color32(147, 147, 147, 89);
    }
}
