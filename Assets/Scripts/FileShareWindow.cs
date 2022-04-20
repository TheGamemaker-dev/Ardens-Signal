using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileShareWindow : MonoBehaviour
{
    //Can download files from others IF have not downloaded before
    //Can send files from paint or file system to others
    //can switch between shared files and incoming files

    [SerializeField] SharedFile[] files;

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

    }
}

public class SharedFile
{
    public enum SharedFileType { Singal, Paint, Other };
    public SharedFileType type;
    public string name;
    public string[] flagsNeeded; //the flags needed for the file to show in the window
    internal bool downloaded;
}