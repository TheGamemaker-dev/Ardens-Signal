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

    List<MessageGroup> allMessages = new List<MessageGroup>();
    ChatWindow chatWindow;
    CutsceneManager cutsceneManager;
    AudioManager audioManager;

    public static GameManager singleton;
    public static UnityAction<string> onFlagSet;

    public string playerName;
    public bool isNewGame = false;

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
        }
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
        Debug.Log(builder.ToString());
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
                TextAsset[] allMGFiles = Resources.LoadAll<TextAsset>("Message Groups");
                foreach (TextAsset groupFile in allMGFiles)
                {
                    MessageGroup group = MessageGroupCompiler.Compile(groupFile);

                    allMessages.Add(group);
                }
                cutsceneManager = FindObjectOfType<CutsceneManager>();
                StartCoroutine(cutsceneManager.StartCutscene(1));
                audioManager = FindObjectOfType<AudioManager>();
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
            jsonWriter.WriteStartArray();
            jsonWriter.WritePropertyName(log.name);
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
        FileStream file = File.Create(Application.persistentDataPath + "/SaveData.dat");
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game Saved!");
    }

    public void Load() { }

    public static void Quit()
    {
        Application.Quit();
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
}
