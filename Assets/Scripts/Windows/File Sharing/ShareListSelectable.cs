using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareListSelectable : Selectable
{
    // Start is called before the first frame update
    ListButtons peopleList;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        peopleList = transform.parent.gameObject.GetComponentInChildren<ListButtons>();
    }


    public void Selected()
    {
        peopleList.ShowList(gameObject.name);
    }
}
