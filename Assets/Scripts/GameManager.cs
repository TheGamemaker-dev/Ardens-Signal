using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static Dictionary<string, bool> flags = new Dictionary<string, bool>() {
        {"logIn", false}, {"helped", false}
    };

    List<MessageGroup> allMessages = new List<MessageGroup>();

    static GameManager singleton;


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

    void Start()
    {
        TextAsset[] allMGFiles = Resources.LoadAll<TextAsset>("Message Groups");
        foreach (TextAsset groupFile in allMGFiles)
        {
            allMessages.Add(MessageGroupCompiler.Compile(groupFile));
        }
        Debug.Log(Time.deltaTime);
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
                if (flags.ContainsKey(instParams[1]))
                {
                    flags[instParams[1]] = true;
                }
                else
                {
                    Debug.LogError("No flag found named " + instParams[1]);
                }
                break;
        }
    }
}
