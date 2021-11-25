using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static Dictionary<string, bool> flags = new Dictionary<string, bool>() {
        {"logIn", false}, {"helped", false}
    };

    List<MessageGroup> allMessages = new List<MessageGroup>();
    ChatWindow chatWindow;

    static GameManager singleton;
    static UnityAction<string> onFlagSet;


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
    }

    void OnEnable()
    {
        onFlagSet += StartMessageGroups;
    }
    void OnDisable()
    {
        onFlagSet -= StartMessageGroups;
    }

    void Start()
    {
        TextAsset[] allMGFiles = Resources.LoadAll<TextAsset>("Message Groups");
        foreach (TextAsset groupFile in allMGFiles)
        {
            allMessages.Add(MessageGroupCompiler.Compile(groupFile));
        }
        chatWindow = FindObjectOfType<ChatWindow>();
        SetFlag("logIn");
    }

    public static void ExecuteInstruction(string instruction)
    {
        string[] instParams = instruction.Split(':');
        switch (instParams[0])
        {
            case "FROM":
                break;
            case "FLAGSREQUIRED":
                break;
            case "STOP":
                break;
            case "SETFLAG":
                SetFlag(instParams[1]);
                break;
        }
    }
    static void SetFlag(string flag)
    {
        if (flags.ContainsKey(flag))
        {
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
                if (flags[flagNeeded] == false) //if at least one flag is false
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
}
