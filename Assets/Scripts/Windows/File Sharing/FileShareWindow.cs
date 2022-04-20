using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FileShareWindow : MonoBehaviour
{
    //Can download files from others IF have not downloaded before
    //Can send files from paint or file system to others
    //can switch between shared files and incoming files

    [SerializeField] SharedFile[] incomingFiles;
    [SerializeField] Sprite[] fileImages;
    [SerializeField] GameObject sharedFileParent, incomingFilesParent;
    [SerializeField] GameObject sharedFilePrefab;

    void Start()
    {
        if (fileImages.Length != Enum.GetNames(typeof(SharedFile.SharedFileType)).Length)
        {
            Debug.LogError("fileImages length does not match SharedFileTypes length");
        }
    }

    void OnEnable()
    {
        GameManager.onFlagSet += UpdateFileList;
    }

    void OnDisable()
    {
        GameManager.onFlagSet -= UpdateFileList;
    }

    void UpdateFileList(string flagSet)
    {
        foreach (SharedFile file in incomingFiles)
        {
            if (!file.visible && flagSet == file.flagNeeded)
            {
                GameObject fileInstance = Instantiate(sharedFilePrefab, incomingFilesParent.transform);
                fileInstance.GetComponent<Image>().sprite = fileImages[((int)file.type)];
                fileInstance.GetComponentInChildren<Text>().text = file.name;
                file.visible = true;
            }
        }
    }
}

[Serializable]
public class SharedFile
{
    public enum SharedFileType { Singal, Paint, Other };
    public SharedFileType type;
    public string name;
    public string flagNeeded; //the flags needed for the file to show in the window
    internal bool downloaded, visible;
}