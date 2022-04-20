using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileList : MonoBehaviour
{
    List<GameObject> dialogueWindows = new List<GameObject>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Files Window"))
            {
                dialogueWindows.Add(child.gameObject);
            }
        }
    }
    public void ShowList(string name)
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
