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

    public Sprite[] fileImages;
    public AnimationClip downloadAnim;

    [SerializeField] IncomingFile[] incomingFiles;
    [SerializeField] GameObject sharedFileParent, incomingFilesParent;
    [SerializeField] GameObject sharedFilePrefab;

    void Start()
    {
        if (fileImages.Length != Enum.GetNames(typeof(IncomingFile.SharedFileType)).Length)
        {
            Debug.LogError("fileImages length does not match SharedFileTypes length");
        }
        UpdateFileList("signalSent");
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
        foreach (IncomingFile file in incomingFiles)
        {
            if (!file.visible && flagSet == file.flagNeeded)
            {
                IncomingFileUIInstance fileInstance = Instantiate(sharedFilePrefab, incomingFilesParent.transform).GetComponent<IncomingFileUIInstance>();
                fileInstance.SetFileRep(file);
            }
        }
        incomingFilesParent.GetComponent<Text>().text = incomingFilesParent.transform.childCount == 0 ? "No files have been shared with you, check back later!" : "";
    }
}

[Serializable]
public class IncomingFile
{
    public enum SharedFileType { Singal, Paint, Other };
    public SharedFileType type;
    public string name, downloadedFlag;
    public string flagNeeded = null; //the flags needed for the file to show in the window
    internal bool downloaded, visible;
}