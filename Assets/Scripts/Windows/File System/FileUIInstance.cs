using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileUIInstance : MonoBehaviour
{
    DoubleClickHandler handler;
    File curFile;

    private void Awake()
    {
        handler = GetComponent<DoubleClickHandler>();
    }

    private void OnEnable()
    {
        handler.doubleClicked += DoAction;
    }

    private void OnDisable()
    {
        handler.doubleClicked -= DoAction;
    }

    public void SetFileRep(File file)
    {
        curFile = file;
        GetComponent<Image>().sprite = FindObjectOfType<FileSystemWindow>().fileImages[
            (int)file.type
        ];
        GetComponentInChildren<Text>().text = file.name;
    }

    void DoAction()
    {
        curFile.onOpened?.Invoke();
    }
}
