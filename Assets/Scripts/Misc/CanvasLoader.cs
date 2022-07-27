using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Cutscenes")
            {
                continue;
            }
            child.gameObject.SetActive(false);
        }
        StartCoroutine(LoadInTime());
    }

    IEnumerator LoadInTime()
    {
        yield return new WaitForSeconds(.25f);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
