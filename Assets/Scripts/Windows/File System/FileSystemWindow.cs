using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FileSystemWindow : MonoBehaviour
{
    public Sprite[] fileImages;

    [SerializeField]
    File[] files;

    [SerializeField]
    Transform[] fileLists;

    [SerializeField]
    GameObject fileUIPrefab;

    private void OnEnable()
    {
        GameManager.onFlagSet += UpdateFileLists;
    }

    private void OnDisable()
    {
        GameManager.onFlagSet -= UpdateFileLists;
    }

    void UpdateFileLists(string flagSet)
    {
        foreach (File file in files)
        {
            if (!file.visible && file.flagNeeded == flagSet)
            {
                Instantiate(
                    fileUIPrefab,
                    fileLists[(int)file.folder]
                        .GetComponentInChildren<VerticalLayoutGroup>()
                        .transform
                );
            }
        }
    }
}

[Serializable]
public class File
{
    public enum FileType
    {
        Signal,
        Other
    };

    public enum FileFolder
    {
        Downloads,
        Paint,
        Chatroom
    };

    public string name;
    public FileType type;
    public FileFolder folder;
    public string flagNeeded;
    public UnityEvent onOpened;
    internal bool visible;
}
