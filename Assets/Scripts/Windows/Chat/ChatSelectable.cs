using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatSelectable : Selectable
{

    public Text status;

    PeopleList peopleList;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        peopleList = FindObjectOfType<PeopleList>();
    }


    public void Selected()
    {
        peopleList.ShowConversation(gameObject.name);
    }
}
