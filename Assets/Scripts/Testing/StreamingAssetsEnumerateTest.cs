using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class StreamingAssetsEnumerateTest : MonoBehaviour
{
    private void Start()
    {
        List<string> possiblePaths = Directory
            .EnumerateFiles(
                Application.streamingAssetsPath,
                "*Day 1 Group 1*",
                SearchOption.AllDirectories
            )
            .ToList();

        string path = possiblePaths[0];

        string content = File.ReadAllText(path);

        Debug.Log(content);
    }
}
