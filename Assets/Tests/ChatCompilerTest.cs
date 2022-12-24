using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChatCompilerTest
{
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
        Debug.Assert(expected.Equals(actual));
    }

    string GetChatFile(string name)
    {
		string output;
		try
        {
            string path = Application.dataPath + "/Tests/Test Chat Files/" + name + ".txt";
            output = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            output = "-1";
        }
        return output;
    }

}
