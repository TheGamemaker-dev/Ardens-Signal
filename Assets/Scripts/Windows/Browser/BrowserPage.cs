using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserPage : MonoBehaviour
{
    public string url;

    BrowserWindow window;

    // Start is called before the first frame update
    void Start()
    {
        window = FindObjectOfType<BrowserWindow>();
    }

    public void ChangePage(BrowserPage page)
    {
        window.ChangePage(page);
    }
}
