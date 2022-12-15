using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;

public class MessageGroup : IEquatable<MessageGroup>
{
    public Dictionary<int, string> jumps { get; }
    public Dictionary<int, Message> messages { get; }
    public Dictionary<int, string> instructions { get; }
    public Dictionary<int, string> lineTypes { get; }
    public string from { get; }
    public string[] flagsRequired { get; }
    public bool triggered { get; set; }
    public string name;

    public MessageGroup(
        Dictionary<int, string> jumps,
        Dictionary<int, Message> messages,
        Dictionary<int, string> instructions,
        Dictionary<int, string> lineTypes,
        string from,
        string[] flagsRequired,
        string name
    )
    {
        this.jumps = jumps;
        this.messages = messages;
        this.instructions = instructions;
        this.lineTypes = lineTypes;
        this.from = from;
        this.flagsRequired = flagsRequired;
        this.triggered = false;
        this.name = name;
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

    public Message NextMessage(Message lastMessage)
    {
        //check if theres a jump
        int nextIndex = 0;
        int lastMessageIndex = messages.FirstOrDefault(x => x.Value == lastMessage).Key;
        if (lastMessage.jumpTo != null)
        {
            int jumpIndex = jumps.FirstOrDefault(x => x.Value == lastMessage.jumpTo).Key;
            nextIndex = jumpIndex;
        }
        else if (lastMessage.choices != null)
        {
            nextIndex = lastMessageIndex + lastMessage.choices.Length;
        }
        else
        {
            //if no jump, check the next line

            nextIndex = lastMessageIndex;
        }

        return GetNextMessageFromLine(nextIndex);
    }

    public Message NextMessage(Message message, int choiceIndex)
    {
        if (message.choices == null)
        {
            Debug.LogWarning("Wrong NextMessage used, with choice");
            return NextMessage(message);
        }

        Choice choiceChosen = message.choices[choiceIndex];
        string jump = choiceChosen.jumpTo;
        int jumpIndex;

        if (jump == "")
        {
            jumpIndex =
                messages.FirstOrDefault(x => x.Value == message).Key + message.choices.Length;
        }
        else
        {
            jumpIndex = jumps.FirstOrDefault(x => x.Value == jump).Key;
        }

        return GetNextMessageFromLine(jumpIndex);
    }

    Message GetNextMessageFromLine(int lineNum)
    {
        int nextIndex = lineNum + 1;
        while (true)
        {
            string nextLineType = lineTypes[nextIndex];

            switch (nextLineType)
            {
                case "instruction":
                    GameManager.singleton.ExecuteInstruction(instructions[nextIndex]);
                    if (instructions[nextIndex] == "STOP")
                    {
                        goto Stop;
                    }
                    nextIndex++;
                    break;
                case "message":
                    return messages[nextIndex];
                default:
                    throw new UnityException("Unknown line type: " + nextLineType);
            }
        }
        Stop:
        return new Message();
    }

    public bool Equals(MessageGroup other)
    {
        if (other == null)
        {
            return false;
        }

        bool areEqual = jumps.IsEqualTo(other.jumps);
        areEqual &= messages.IsEqualTo(other.messages);
        areEqual &= instructions.IsEqualTo(other.instructions);
        areEqual &= lineTypes.IsEqualTo(other.lineTypes);
        areEqual &= from == other.from;
        areEqual &= flagsRequired.IsEqualTo(other.flagsRequired);
        areEqual &= triggered == other.triggered;
        areEqual &= name == other.name;

        return areEqual;
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
