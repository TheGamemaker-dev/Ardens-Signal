using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatSelectable : Selectable
{

    public Text status;

    ListButtons peopleList;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        peopleList = GetComponentInParent<ListButtons>();
    }


    public void Selected()
    {
        peopleList.ShowList(gameObject.name);
    }
}
