using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ChatRunTest : MonoBehaviour
{
    TextAsset asset;

    // Start is called before the first frame update
    void Start()
    {
        asset = new TextAsset(
            File.ReadAllText(
                Application.streamingAssetsPath + "/Message Groups/Day 1/Arden Day 1 Group 1.chat"
            )
        );
        ChatWindow chatWindow = FindObjectOfType<ChatWindow>();
        MessageGroup messageGroup = MessageGroupCompiler.Compile(asset);
        chatWindow.StartMessageGroup(messageGroup);
    }
}
