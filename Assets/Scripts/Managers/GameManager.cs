using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

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

    List<MessageGroup> allMessages = new List<MessageGroup>();
    ChatWindow chatWindow;
    CutsceneManager cutsceneManager;
    AudioManager audioManager;

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
            CompileMessages();
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
                chatWindow = FindObjectOfType<ChatWindow>();
                audioManager = FindObjectOfType<AudioManager>();
                cutsceneManager = FindObjectOfType<CutsceneManager>();

                StartCoroutine(cutsceneManager.StartCutscene(1));
                GameObject.Find("Save Button").GetComponent<Button>().onClick.AddListener(Save);
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

    void StartMessageGroups(string flagSet)
    {
        foreach (MessageGroup group in allMessages)
        {
            bool canTrigger = true;
            foreach (string flagNeeded in group.flagsRequired)
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
                chatWindow.StartMessageGroup(group);
            }
        }
    }

    public void Save()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        {
            Debug.LogError("Save should only be called in the Game scene");
            return;
        }
        //things to be saved: flags, chat logs
        //chatLogs
        StringBuilder builder = new StringBuilder();
        StringWriter writer = new StringWriter(builder);
        JsonTextWriter jsonWriter = new JsonTextWriter(writer);
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
        string messageJson = builder.ToString();

        GameData data = new GameData(flags, messageJson);

        //save data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFile);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game Saved!");
    }

    public void Load()
    {
        bool dataExists = File.Exists(saveFile);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(saveFile, FileMode.Open);
        GameData data = (GameData)bf.Deserialize(file);

        //convert flag json data to dictionary
        flags = data.GetFlags();

        //set message groups as triggered
        foreach (MessageGroup group in allMessages)
        {
            bool triggered = true;
            foreach (string flag in group.flagsRequired)
            {
                bool needsTrue = flag[0] != '!';
                string curFlag = needsTrue ? flag : flag.Remove(0, 1);

                if (flags[curFlag] != needsTrue)
                {
                    triggered = false;
                }
            }

            group.triggered = triggered;
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }

    void CompileMessages()
    {
        TextAsset[] allMGFiles = Resources.LoadAll<TextAsset>("Message Groups");

        foreach (TextAsset groupFile in allMGFiles)
        {
            MessageGroup group = MessageGroupCompiler.Compile(groupFile);

            allMessages.Add(group);
        }
    }
}

[Serializable]
public class GameData
{
    string flagsJsonData;
    string chatJsonData;

    public GameData(Dictionary<string, bool> flags, string chatJsonData)
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

        this.chatJsonData = chatJsonData;
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
                Debug.Log("Token: " + token + "\t Value: " + value);
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
                else
                {
                    Debug.Log(token);
                }
            }
            else
            {
                Debug.Log("Token: " + jsonReader.TokenType);
            }
        }
        return output;
    }
}
