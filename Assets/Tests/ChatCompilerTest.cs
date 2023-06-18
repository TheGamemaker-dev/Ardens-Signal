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
    public void EmptyFileTest() { }

    [Test]
    public void FromInstTest() { }

    [Test]
    public void FlagsTest() { }

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
        return false;
    }
    #endregion
}
