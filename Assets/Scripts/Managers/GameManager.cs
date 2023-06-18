using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    public static UnityAction<int> onDayEnd;

    internal string playerName;
    internal bool isNewGame = false;

    ChatWindow chatWindow;
    CutsceneManager cutsceneManager;
    AudioManager audioManager;

    public bool GetFlagState(string flag)
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

    public void SetFlag(string flag)
    {
        if (flags.ContainsKey(flag))
        {
            Debug.Log("Flag set: " + flag);
            flags[flag] = true;
            onFlagSet?.Invoke(flag);

            Regex reg = new Regex(@"day([0-9])*Done");
            Match match = reg.Match(flag);
            if (match.Success)
            {
                int day = int.Parse(match.Groups[1].Value);
                onDayEnd?.Invoke(day);
            }
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
            GameDataManager.GetAllMessageGroupPreData();
        }
    }

    void OnEnable()
    {
        onFlagSet += StartMessageGroups;
        SceneManager.sceneLoaded += OnSceneLoaded;
        onDayEnd += AutoSave;
    }

    void OnDisable()
    {
        onFlagSet -= StartMessageGroups;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        onDayEnd -= AutoSave;
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
        //Display all the messages in the correct order
        chatWindow.MassMessageSend(GameDataManager.savedData.ChatJsonData);
        //Determine which message groups to start and where to start them
        SetFlag("day" + (GameDataManager.savedData.LastDay + 1) + "Start");
    }

    void AutoSave(int day)
    {
        GameDataManager.Save(chatWindow, day);
    }

    private void StartMessageGroups(string flagSet)
    {
        foreach (MessageGroupPreData group in GameDataManager.messageGroupPreData)
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

    public string GetVariable(string variableName)
    {
        switch (variableName)
        {
            case "playerName":
                return playerName;
            default:
                return "";
        }
    }
}
