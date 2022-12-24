using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Dictionary<string, bool> flags = new Dictionary<string, bool>()
    {
        { "logIn", false },
        { "downloadAi", false },
        { "aiDownloaded", false },
        { "aiInstalled", false },
        { "aiSetup", false },
        { "day1Done", false },
        { "day2Start", false },
        { "day2Done", false },
        { "day3Start", false },
        { "askedOut", false },
        { "aiRestart", false },
        { "dream", false },
        { "8ball", false }
    };

    public static GameManager singleton;
    public static UnityAction<string> onFlagSet;
    public static string saveFile;

    public string playerName;
    public bool isNewGame = false;

    static GameData savedData;

	readonly List<MessageGroupPreData> messageGroupPreData = new List<MessageGroupPreData>();

    ChatWindow chatWindow;
    CutsceneManager cutsceneManager;
    AudioManager audioManager;

    public static bool GetFlagState(string flag)
    {
        if (flags.Keys.Contains(flag))
        {
            return flags[flag];
        }
        else
        {
            return false;
        }
    }

    public static void SetFlag(string flag)
    {
        if (flags.ContainsKey(flag))
        {
            Debug.Log("Flag set: " + flag);
            flags[flag] = true;
            onFlagSet?.Invoke(flag);
        }
        else
        {
            Debug.LogError("No flag found named " + flag);
        }
    }

    void Awake()
    {
        if (singleton && singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            saveFile = Application.persistentDataPath + "/SaveData.dat";
            GetAllMessageGroupPreData();
        }
    }

    void OnEnable()
    {
        onFlagSet += StartMessageGroups;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        onFlagSet -= StartMessageGroups;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        switch (scene.name)
        {
            case "Game":
                //general setup
                chatWindow = FindObjectOfType<ChatWindow>();
                audioManager = FindObjectOfType<AudioManager>();
                cutsceneManager = FindObjectOfType<CutsceneManager>();

                if (isNewGame)
                { //new game only
                    FindObjectOfType<LoginManager>().gameObject.transform.SetAsLastSibling();
                    StartCoroutine(cutsceneManager.StartCutscene(1));
                }
                else
                {
                    ContinueGame();
                }
                break;
            default:
                break;
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExecuteInstruction(string instruction)
    {
        string[] instParams = instruction.Split(':');
        switch (instParams[0])
        {
            case "FROM":
            case "FLAGSREQUIRED":
            case "STOP":
                break;
            case "SETFLAG":
                SetFlag(instParams[1]);
                break;
            case "PLAYSOUND":
                string nameOfSound = instParams[1];
                audioManager.PlaySound(nameOfSound, false);
                break;
        }
    }

    public void Save()
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
        jsonWriter.WriteComment("Load-bearing dummy data!");

        string messageJson = builder.ToString();

        GameData data = new GameData(flags, messageJson, playerName);

        //save data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFile);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game Saved!");
    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(saveFile, FileMode.Open);
        savedData = (GameData)bf.Deserialize(file);

        //convert flag json data to dictionary
        flags = savedData.GetFlags();

        //set message groups as triggered
        foreach (MessageGroupPreData group in messageGroupPreData)
        {
            bool triggered = true;
            foreach (string flag in group.flags)
            {
                bool needsTrue = flag[0] != '!';
                string curFlag = needsTrue ? flag : flag.Remove(0, 1);

                if (flags[curFlag] != needsTrue)
                {
                    triggered = false;
                    break;
                }
            }

            group.triggered = triggered;
        }
        playerName = savedData.PlayerName;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }

    private void ContinueGame()
    {
        foreach (MessageGroupPreData messageGroup in messageGroupPreData)
        {
            if (messageGroup.triggered)
            {
                chatWindow.lastGroupsTriggered.Add(MessageGroup.GetGroupFromPreData(messageGroup));
            }
        }
        //Unpack and display chat data
        JsonReader reader = new JsonTextReader(new StringReader(savedData.ChatJsonData));
        GameObject curDWindow = null;
        string lastValue = "";
        string curProp = "",
            lastProp = "";
        while (reader.Read())
        {
#nullable enable
            object? curValue = reader.Value;
#nullable disable
            JsonToken curToken = reader.TokenType;
            if (curValue != null)
            {
                switch (curToken)
                {
                    case JsonToken.PropertyName:
                    case JsonToken.Comment: //At the end or start of a chat log of a chat log
                        if (curProp != curValue.ToString())
                        {
                            lastProp = curProp;
                            curProp = curValue.ToString();
                        }
                        if (lastValue != "") //At the end of a chat log, pick up the chat from where it left off
                        {
                            //Find the possible chats we could be starting from
                            List<MessageGroup> groupsToStart = chatWindow.lastGroupsTriggered
                                .Where(x => x.from == lastProp)
                                .ToList();
                            MessageGroup groupToStart = null;

                            foreach (MessageGroup group in groupsToStart)
                            {
                                foreach (Message message in group.messages.Values)
                                {
                                    if (message.message == lastValue)
                                    {
                                        groupToStart = group;
                                        break;
                                    }
                                }
                                if (groupToStart != null)
                                {
                                    break;
                                }
                            }
                            //Find the index we're starting from and start from there
                            int indexToStart = groupToStart.messages
                                .First(x => x.Value.message == lastValue)
                                .Key;
                            chatWindow.StartMessageGroup(groupToStart, indexToStart);
                        }
                        if (curToken == JsonToken.PropertyName) //At the start of a chat log
                        {
                            curDWindow = chatWindow.dialogueWindows[curValue.ToString()];
                        }
                        break;
                    case JsonToken.String: //At a chat message
                        chatWindow.SendMessageImmediate(
                            curDWindow,
                            curValue.ToString()[0] == '_',
                            curValue.ToString()
                        );
                        lastValue = curValue.ToString();
                        break;
                    default:
                        Debug.LogWarning("Unknown Token: " + curToken);
                        break;
                }
            }
        }
        audioManager.PlaySound("Fan Whirring", true);
    }

    private void StartMessageGroups(string flagSet)
    {
        foreach (MessageGroupPreData group in messageGroupPreData)
        {
            if (!group.flags.Contains(flagSet))
            {
                continue;
            }
            bool canTrigger = true;
            foreach (string flagNeeded in group.flags)
            {
                string flag = flagNeeded;
                bool mustBeTrue = true;
                if (flag[0] == '!')
                {
                    mustBeTrue = false;
                    flag = flag.Substring(1);
                }

                if (flags[flag] == !mustBeTrue) //if at least one flag is incorrect
                {
                    canTrigger = false;
                }
            }
            if (canTrigger && !group.triggered)
            {
                group.triggered = true;
                chatWindow.StartMessageGroup(MessageGroup.GetGroupFromPreData(group));
            }
        }
    }

    private void GetAllMessageGroupPreData()
    {
        List<string> filePaths = Directory
            .EnumerateFiles(
                Application.streamingAssetsPath + "/Message Groups",
                "*.txt",
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
}

[Serializable]
public class GameData
{
    public string PlayerName { get; set; }
    public string ChatJsonData { get; set; }

	readonly string flagsJsonData;

    public GameData(Dictionary<string, bool> flags, string chatJsonData, string playerName)
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
