using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListButtons : MonoBehaviour
{

    [SerializeField] string listTag;
    [SerializeField] Transform listsParent;

    List<GameObject> lists = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in listsParent)
        {
            if (child.gameObject.CompareTag(listTag))
            {
                lists.Add(child.gameObject);
            }
        }
    }

    public void ShowList(string name)
    {
        foreach (GameObject list in lists)
        {
            list.SetActive(list.name == name);
        }
    }
}
