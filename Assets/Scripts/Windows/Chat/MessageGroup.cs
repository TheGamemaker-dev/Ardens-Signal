using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;

public class MessageGroup
{
    public string from;
    public string[] flagsRequired;
    public bool triggered;
    public string name;
    public Message rootMessage;

    public MessageGroup(string from, string[] flagsRequired, string name, Message rootMessage)
    {
        this.from = from;
        this.flagsRequired = flagsRequired;
        this.triggered = false;
        this.name = name;
        this.rootMessage = rootMessage;
    }

    public MessageGroup()
    {
        from = "";
        flagsRequired = new string[] { };
        triggered = false;
        name = "";
        rootMessage = null;
    }

    public static MessageGroup GetGroupFromPreData(MessageGroupPreData data)
    {
        string filePath = Directory
            .EnumerateFiles(
                Application.streamingAssetsPath + "/Message Groups",
                "*" + data.name + "*",
                SearchOption.AllDirectories
            )
            .First();

        string file = File.ReadAllText(filePath);

        TextAsset asset = new TextAsset(file);
        asset.name = data.name;

        MessageGroup output = MessageGroupCompiler.Compile(asset);

        return output;
    }
}

public class MessageGroupPreData
{
    public bool triggered;
    public string name;
    public string[] flags { get; private set; }

    public MessageGroupPreData(string name, string[] flags)
    {
        this.name = name;
        this.flags = flags;
        triggered = false;
    }
}
