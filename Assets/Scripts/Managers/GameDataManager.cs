using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public static class GameDataManager
{
    public static readonly string saveFile = Application.persistentDataPath + "/save.dat";
    public static GameData savedData { get; private set; }
    public static readonly List<MessageGroupPreData> messageGroupPreData =
        new List<MessageGroupPreData>();

    public static void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(saveFile, FileMode.Open);
        savedData = (GameData)bf.Deserialize(file);

        //convert flag json data to dictionary
        GameManager.flags = savedData.GetFlags();

        //set message groups as triggered
        foreach (MessageGroupPreData group in messageGroupPreData)
        {
            bool triggered = true;
            foreach (string flag in group.flags)
            {
                bool needsTrue = flag[0] != '!';
                string curFlag = needsTrue ? flag : flag.Remove(0, 1);

                if (GameManager.flags[curFlag] != needsTrue)
                {
                    triggered = false;
                    break;
                }
            }

            group.triggered = triggered;
        }
        GameManager.singleton.playerName = savedData.PlayerName;
    }

    public static void GetAllMessageGroupPreData()
    {
        List<string> filePaths = Directory
            .EnumerateFiles(
                Application.streamingAssetsPath + "/Message Groups",
                "*.chat",
                SearchOption.AllDirectories
            )
            .ToList();
        foreach (string path in filePaths)
        {
            string[] pathList = path.Split(
                new char[] { '/', '\\' },
                StringSplitOptions.RemoveEmptyEntries
            );
            string name = pathList[pathList.Length - 1].Split('.')[0];

            string[] contents = File.ReadAllText(path).Split('\n');
            string[] flags = new string[0];

            foreach (string line in contents)
            {
                if (line.Contains("FLAGSREQUIRED"))
                {
                    flags = line.Split(':')[1].Split(',');
                    for (int i = 0; i < flags.Length; i++)
                    {
                        flags[i] = flags[i].RemoveWhitespace();
                    }
                    break;
                }
            }

            MessageGroupPreData preData = new MessageGroupPreData(name, flags);
            messageGroupPreData.Add(preData);
        }
    }

    public static void Save(ChatWindow chatWindow, int lastDay)
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Debug.LogError("Save should only be called in the Game scene");
            return;
        }
        //things to be saved: flags, chat logs, player name, last message index
        //chatLogs
        StringBuilder builder = new StringBuilder();
        StringWriter writer = new StringWriter(builder);
        JsonTextWriter jsonWriter = new JsonTextWriter(writer);

        //Format the existing chats in json form
        jsonWriter.WriteStartObject();
        foreach (GameObject log in chatWindow.dialogueWindows.Values)
        {
            Text[] messages = log.GetComponentInChildren<ScrollRect>()
                .GetComponentsInChildren<Text>();
            jsonWriter.WritePropertyName(log.name);
            jsonWriter.WriteStartArray();
            foreach (Text message in messages)
            {
                bool fromYou = message.alignment == TextAnchor.MiddleRight;
                string messageText = (fromYou ? "_" : "") + message.text;
                jsonWriter.WriteValue(messageText);
            }
            jsonWriter.WriteEndArray();
        }
        jsonWriter.WriteEndObject();

        string messageJson = builder.ToString();

        GameData data = new GameData(
            GameManager.flags,
            messageJson,
            GameManager.singleton.playerName,
            lastDay
        );

        //save data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFile);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game Saved!");
    }

    [MenuItem("Tools/Reset Save")]
    public static void DeleteSave()
    {
        File.Delete(saveFile);
    }
}

[Serializable]
public class GameData
{
    public string PlayerName { get; set; }
    public string ChatJsonData { get; set; }
    public int LastDay { get; set; }

    readonly string flagsJsonData;

    public GameData(
        Dictionary<string, bool> flags,
        string chatJsonData,
        string playerName,
        int lastDay
    )
    {
        StringBuilder builder = new StringBuilder();
        StringWriter writer = new StringWriter(builder);
        JsonTextWriter jsonWriter = new JsonTextWriter(writer);
        jsonWriter.WriteStartObject();
        foreach (KeyValuePair<string, bool> flag in flags)
        {
            jsonWriter.WritePropertyName(flag.Key);
            jsonWriter.WriteValue(flag.Value);
        }
        jsonWriter.WriteEndObject();

        flagsJsonData = builder.ToString();

        this.ChatJsonData = chatJsonData;
        this.PlayerName = playerName;
        this.LastDay = lastDay;
    }

    public Dictionary<string, bool> GetFlags()
    {
        Dictionary<string, bool> output = new Dictionary<string, bool>();

        StringReader stringReader = new StringReader(flagsJsonData);
        JsonReader jsonReader = new JsonTextReader(stringReader);

        string lastProp = "";

        while (jsonReader.Read())
        {
            JsonToken token = jsonReader.TokenType;
#nullable enable
            object? value = jsonReader.Value;
#nullable disable
            if (value != null)
            {
                if (token == JsonToken.PropertyName)
                {
                    lastProp = value.ToString();
                }
                else if (token == JsonToken.Boolean)
                {
                    if (!output.ContainsKey(lastProp) && lastProp != "")
                    {
                        output.Add(lastProp, (bool)value);
                    }
                    else
                    {
                        Debug.LogWarning("Value already exists: " + lastProp);
                    }
                }
            }
        }
        return output;
    }
}
