using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FileSystemWindow : MonoBehaviour
{
    public Sprite[] fileImages;

    [SerializeField] File[] files;
    [SerializeField] Transform[] fileLists;
    [SerializeField] GameObject FileUIPrefab;

    void UpdateFileLists(string flagSet)
    {
        foreach (File file in files)
        {
            if (!file.visible && file.flagNeeded == flagSet)
            {
                Instantiate(FileUIPrefab, fileLists[(int)file.folder]);
            }
        }
    }
}

[Serializable]
public class File
{

    public enum FileType { Signal, Other };
    public enum FileFolder { Downloads, Paint, Chatroom };

    public string name;
    public FileType type;
    public FileFolder folder;
    public string flagNeeded;
    public UnityEvent onOpened;
    internal bool visible;
}
