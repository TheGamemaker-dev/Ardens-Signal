using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class ChatWindow : MonoBehaviour, IPointerDownHandler
{
    public Dictionary<string, GameObject> dialogueWindows = new Dictionary<string, GameObject>();

    internal bool wasLastClickedOn = false;

    [SerializeField]
    GameObject messageFromThemPrefab,
        messageFromYouPrefab;

    [SerializeField]
    GameObject optionPrefab;

    Dictionary<string, ChatSelectable> chatSelectables = new Dictionary<string, ChatSelectable>();
    bool wasClickedOn = false;
    AudioManager audioManager;

    void OnEnable()
    {
        GameManager.onFlagSet += UpdateSelectables;
    }

    void Awake()
    {
        //Get all dialogue windows
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Dialogue Window"))
            {
                dialogueWindows.Add(child.gameObject.name, child.gameObject);
            }
        }

        //get all selectables
        ChatSelectable[] selectables = FindObjectsOfType<ChatSelectable>();
        foreach (ChatSelectable selectable in selectables)
        {
            chatSelectables.Add(selectable.gameObject.name, selectable);
        }

        //set selectable chats based on flags
        foreach (string flag in GameManager.flags.Keys)
        {
            UpdateSelectables(flag);
        }

        //Get AudioManager
        audioManager = FindObjectOfType<AudioManager>();
    }

    void UpdateSelectables(string flag)
    {
        //Certain selectables require flags to appear in the chat
        switch (flag)
        {
            case "aiInstalled":
                chatSelectables["Signal"].gameObject.SetActive(
                    GameManager.singleton.GetFlagState(flag)
                );
                break;
            default:
                break;
        }
    }

    Transform GetContentObject(GameObject window)
    {
        Transform[] children = window.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name == "Dialogue Content")
            {
                return child;
            }
        }
        return null;
    }

    //keeping track of if the window was the last thing to be clicked
    public void OnPointerDown(PointerEventData data)
    {
        wasLastClickedOn = true;
        wasClickedOn = true;
    }

    void Update()
    {
        if (!wasClickedOn && Input.GetMouseButtonDown(0))
        {
            wasLastClickedOn = false;
        }
        wasClickedOn = false;
    }

    public void StartMessageGroup(MessageGroup group, int startMessageIndex = 0)
    {
        Debug.Log("Starting group: " + group.name);
        GameObject dWindow = dialogueWindows[group.from];
        GameObject dContent = GetContentObject(dWindow).gameObject;
        Transform[] children = dWindow.GetComponentsInChildren<Transform>();

        GameObject optionsParent = children.First(x => x.CompareTag("Choice Parent")).gameObject;

        SetStatus(group.from, true);

        //StartCoroutine SendMessage given group, and only stop if choice needs making or end of file reached
        StartCoroutine(StartDialog(group, dContent, optionsParent));
    }

    /// <summary>
    /// Start a message group in the chat window
    /// </summary>
    /// <param name="group">Group to start</param>
    /// <param name="dContent">The dialog content object that the message objects will be instantiated under</param>
    IEnumerator StartDialog(MessageGroup group, GameObject dContent, GameObject optionsParent)
    {
        Message currentMessage = group.rootMessage;

        do
        {
            if (currentMessage.message != "")
            {
                SendMessageImmediate(dContent, currentMessage.fromYou, currentMessage.message);
            }
            if (currentMessage.playSound != "")
            {
                AudioManager.singleton.PlaySound(currentMessage.playSound, false);
            }
            if (currentMessage.setFlag != "")
            {
                GameManager.singleton.SetFlag(currentMessage.setFlag);
            }

            if (currentMessage.choices[0].choiceMessage != "")
            {
                foreach (Choice choice in currentMessage.choices)
                {
                    Choice curChoice = choice;
                    GameObject option = Instantiate(optionPrefab, optionsParent.transform);
                    option.GetComponentInChildren<Text>().text = curChoice.choiceMessage;
                    option
                        .GetComponent<Button>()
                        .onClick.AddListener(() =>
                        {
                            currentMessage = curChoice.jumpTo;
                            SendMessageImmediate(dContent, true, curChoice.choiceMessage);
                            foreach (Transform child in optionsParent.transform)
                            {
                                Destroy(child.gameObject);
                            }
                        });
                }
                yield return new WaitWhile(() => optionsParent.transform.childCount > 0);
            }
            else
            {
                currentMessage = currentMessage.choices[0].jumpTo;
            }

            yield return new WaitForSeconds(1.5f);
        } while (currentMessage.choices[0] != null);
        SetStatus(group.from, false);
    }

    public void SendMessageImmediate(GameObject dialogWindow, bool fromYou, string message)
    {
        GameObject objectToInstantiate = fromYou ? messageFromYouPrefab : messageFromThemPrefab;

        Text messageTextComponent = Instantiate(objectToInstantiate, dialogWindow.transform)
            .GetComponent<Text>();
        messageTextComponent.text = message.Replace("_", "  ");
    }

    void SetStatus(string name, bool status)
    {
        chatSelectables[name].status.text = status ? "Online" : "Offline";
        chatSelectables[name].status.color = status ? Color.green : Color.red;
    }

    public void MassMessageSend(string messageData)
    {
        JsonTextReader reader = new JsonTextReader(new StringReader(messageData));
        GameObject dContent = null;

        while (reader.Read())
        {
            JsonToken token = reader.TokenType;
            string value = reader.Value == null ? "" : reader.Value.ToString();

            switch (token)
            {
                case JsonToken.PropertyName:
                    if (dialogueWindows.ContainsKey(value))
                    {
                        dContent = GetContentObject(dialogueWindows[value]).gameObject;
                    }
                    else
                    {
                        Debug.LogError("No dialogue window with name: " + value);
                    }
                    break;
                case JsonToken.String:
                    string message = value;
                    bool fromYou = false;
                    if (message[0] == '_')
                    {
                        fromYou = true;
                        message = message.Substring(1);
                    }
                    if (dContent == null)
                    {
                        Debug.LogError("No dialogue window set");
                        continue;
                    }
                    SendMessageImmediate(dContent, fromYou, message);
                    break;
                default:
                    break;
            }
        }
    }
}
