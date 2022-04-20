using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleList : MonoBehaviour
{

    List<GameObject> dialogueWindows = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in transform.parent)
        {
            if (child.gameObject.CompareTag("Dialogue Window"))
            {
                dialogueWindows.Add(child.gameObject);
            }
        }
    }
    public void ShowConversation(string name)
    {
        foreach (GameObject window in dialogueWindows)
        {
            window.SetActive(false);
            if (window.name == name)
            {
                window.SetActive(true);
            }
        }
    }
}
