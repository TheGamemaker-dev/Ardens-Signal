using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncomingFileUIInstance : MonoBehaviour
{
    [SerializeField]
    Button button;

    IncomingFile file;
    FileShareWindow fileShareWindow;

    // Start is called before the first frame update
    void Start()
    {
        fileShareWindow = FindObjectOfType<FileShareWindow>();
    }

    // Update is called once per frame
    void Update()
    {
        button.interactable = !file.downloaded;
    }

    public void SetFileRep(IncomingFile newFile)
    {
        file = newFile;
        GetComponent<Image>().sprite = FindObjectOfType<FileShareWindow>().fileImages[
            (int)file.type
        ];
        GetComponentInChildren<Text>().text = file.name;
    }

    public void DownloadFile()
    {
        GetComponent<Animator>().SetTrigger("download");
        file.downloaded = true;
        if (file.downloadedFlag != "")
        {
            Invoke("SetDownloadedFlag", fileShareWindow.downloadAnim.length);
        }
    }

    void SetDownloadedFlag()
    {
        GameManager.singleton.SetFlag(file.downloadedFlag);
    }
}
