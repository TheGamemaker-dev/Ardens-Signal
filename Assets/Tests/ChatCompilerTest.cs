using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChatCompilerTest
{
#region Tests
    [Test]
    public void EmptyFileTest()
    {
        string emptyFile = GetChatFile("Empty");
        TextAsset asset = new TextAsset(emptyFile);
        MessageGroup actual = MessageGroupCompiler.Compile(asset);
        MessageGroup expected = new MessageGroup(
            new Dictionary<int, string>(),
            new Dictionary<int, Message>(),
            new Dictionary<int, string>(),
            new Dictionary<int, string>(),
            "",
            new string[0],
            ""
        );
        Debug.Assert(TestCompare(expected, actual));
    }

    [Test]
    public void FromInstTest()
    {
        string file = GetChatFile("From");
        TextAsset asset = new TextAsset(file);
        MessageGroup actual = MessageGroupCompiler.Compile(asset);
        MessageGroup expected = new MessageGroup()
        {
            messages = new Dictionary<int, Message>()
            {
                {
                    1,
                    new Message() { message = "Hello!", }
                }
            },
            from = "Arden",
            instructions = new Dictionary<int, string>() { { 0, "FROM:Arden" } },
            lineTypes = new Dictionary<int, string>() { { 0, "instruction" }, { 1, "message" } },
        };

        Debug.Assert(TestCompare(expected, actual));
    }

    [Test]
    public void FlagsTest()
    {
        string file = GetChatFile("Flags");
        TextAsset asset = new TextAsset(file);
        MessageGroup actual = MessageGroupCompiler.Compile(asset);
        MessageGroup expected = new MessageGroup()
        {
            from = "Arden",
            instructions = new Dictionary<int, string>()
            {
                { 0, "FROM:Arden" },
                { 1, "FLAGSREQUIRED:start,end" }
            },
            lineTypes = new Dictionary<int, string>()
            {
                { 0, "instruction" },
                { 1, "instruction" }
            },
            flagsRequired = new string[] { "start", "end" }
        };

        Debug.Assert(TestCompare(expected, actual));
    }

#endregion
#region Helper Methods
    string GetChatFile(string name)
    {
        string output;
        try
        {
            string path = Application.dataPath + "/Tests/Test Chat Files/" + name + ".chat";
            output = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            output = "-1";
        }
        return output;
    }

    bool TestCompare(MessageGroup expected, MessageGroup actual)
    {
        bool output = true;

        foreach (KeyValuePair<int, string> pair in expected.jumps)
        {
            try
            {
                if (actual.jumps[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log(
                        "Jump mismatch: jumps at key "
                            + pair.Key
                            + " do not match.\nExpected: "
                            + pair.Value
                            + "\nActual: "
                            + actual.jumps[pair.Key]
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log("Jump mismatch: key " + pair.Key + " not found in actual.jumps");
            }
        }
        foreach (KeyValuePair<int, string> pair in actual.jumps)
        {
            try
            {
                if (expected.jumps[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log(
                        "Jump mismatch: jumps at key "
                            + pair.Key
                            + " do not match.\nExpected: "
                            + expected.jumps[pair.Key]
                            + "\nActual: "
                            + pair.Value
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log("Jump mismatch: key " + pair.Key + " not found in expected.jumps");
            }
        }

        foreach (KeyValuePair<int, Message> pair in expected.messages)
        {
            try
            {
                if (!pair.Value.Equals(actual.messages[pair.Key]))
                {
                    output = false;
                    Debug.Log(
                        "Message mismatch: messages at key "
                            + pair.Key
                            + " do not match.\nExpected: "
                            + pair.Value
                            + "\nActual: "
                            + actual.messages[pair.Key]
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log("Message mismatch: key " + pair.Key + " not found in actual.messages");
            }
        }
        foreach (KeyValuePair<int, Message> pair in actual.messages)
        {
            try
            {
                if (!pair.Value.Equals(expected.messages[pair.Key]))
                {
                    output = false;
                    Debug.Log(
                        "Message mismatch: messages at key "
                            + pair.Key
                            + " do not match.\nExpected: "
                            + expected.messages[pair.Key]
                            + "\nActual: "
                            + pair.Value
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log("Message mismatch: key " + pair.Key + " not found in expected.messages");
            }
        }

        foreach (KeyValuePair<int, string> pair in expected.instructions)
        {
            try
            {
                if (actual.instructions[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log(
                        "Instruction mismatch: instructions at key " + pair.Key + " do not match"
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log(
                    "Instruction mismatch: key " + pair.Key + " not found in actual.instructions"
                );
            }
        }
        foreach (KeyValuePair<int, string> pair in actual.instructions)
        {
            try
            {
                if (expected.instructions[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log(
                        "Instruction mismatch: instructions at key " + pair.Key + " do not match"
                    );
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log(
                    "Instruction mismatch: key " + pair.Key + " not found in expected.instructions"
                );
            }
        }

        foreach (KeyValuePair<int, string> pair in expected.lineTypes)
        {
            try
            {
                if (actual.lineTypes[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log("LineType mismatch: lineTypes at key " + pair.Key + " do not match");
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log("LineType mismatch: key " + pair.Key + " not found in actual.lineTypes");
            }
        }
        foreach (KeyValuePair<int, string> pair in actual.lineTypes)
        {
            try
            {
                if (expected.lineTypes[pair.Key] != pair.Value)
                {
                    output = false;
                    Debug.Log("LineType mismatch: lineTypes at key " + pair.Key + " do not match");
                }
            }
            catch (KeyNotFoundException)
            {
                output = false;
                Debug.Log(
                    "LineType mismatch: key " + pair.Key + " not found in expected.lineTypes"
                );
            }
        }

        if (expected.from != actual.from)
        {
            output = false;
            Debug.Log("From mismatch. Expected: " + expected.from + " Actual: " + actual.from);
        }

        foreach (string flag in expected.flagsRequired)
        {
            if (!actual.flagsRequired.Contains(flag))
            {
                output = false;
                Debug.Log("FlagsRequired mismatch: missing flag " + flag);
            }
        }
        foreach (string flag in actual.flagsRequired)
        {
            if (!expected.flagsRequired.Contains(flag))
            {
                output = false;
                Debug.Log("FlagsRequired mismatch: missing flag " + flag);
            }
        }

        if (expected.triggered != actual.triggered)
        {
            output = false;
            Debug.Log(
                "Triggered mismatch. Expected: "
                    + expected.triggered
                    + " Actual: "
                    + actual.triggered
            );
        }
        if (expected.name != actual.name)
        {
            output = false;
            Debug.Log("Name mismatch. Expected: " + expected.name + " Actual: " + actual.name);
        }
        return output;
    }
    #endregion
}
