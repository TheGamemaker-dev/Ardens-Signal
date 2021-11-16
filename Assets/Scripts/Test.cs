using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    Button button;

    public TextAsset testFile;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { MessageGroupCompiler.Compile(testFile); });
    }
}
