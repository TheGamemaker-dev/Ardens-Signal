using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour, IPointerDownHandler
{
    public bool wasLastClickedOn = false;
    public Dictionary<string, GameObject> dialogueWindows = new Dictionary<string, GameObject>();
    public List<MessageGroup> lastGroupsTriggered = new List<MessageGroup>();

    [SerializeField]
    GameObject messageFromThemPrefab,
        messageFromYouPrefab;

    [SerializeField]
    GameObject optionPrefab;

    bool wasClickedOn = false;
    Dictionary<string, ChatSelectable> chatSelectables = new Dictionary<string, ChatSelectable>();
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
            case "aiDownloaded":
                chatSelectables["Signal"].gameObject.SetActive(GameManager.flags[flag]);
                break;
            default:
                break;
        }
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
        GameObject dWindow = dialogueWindows[group.from];
        GameObject dContent = null;
        Transform[] children = dWindow.GetComponentsInChildren<Transform>();

        if (lastGroupsTriggered.Where(x => x.from == group.from).Count() == 0)
        {
            lastGroupsTriggered.Add(group);
        }
        else
        {
            int indexToReplace = lastGroupsTriggered.IndexOf(
                lastGroupsTriggered.Where(x => x.from == group.from).First()
            );
            lastGroupsTriggered[indexToReplace] = group;
        }

        foreach (Transform child in children)
        {
            if (child.gameObject.name == "Dialogue Content")
            {
                dContent = child.gameObject;
            }
        }
        chatSelectables[group.from].status.text = "Online";
        chatSelectables[group.from].status.color = Color.green;
        //StartCoroutine SendMessage given group, and only stop if choice needs making or end of file reached
        StartCoroutine(StartDialog(group, dContent, startMessageIndex));
    }

    IEnumerator StartDialog(MessageGroup group, GameObject dContent, int startMessageIndex)
    {
        //if no choices, wait then send next message
        //if choice, stop send messages and wait for input, then wait and send next message
        //if end, stop
        Message currentMessage = null;

        if (startMessageIndex == group.messages.Last().Key)
        {
            yield break;
        }

        if (startMessageIndex != 0)
        {
            currentMessage = group.messages[startMessageIndex];
        }

        NextMessage:
        if (currentMessage == null)
        {
            currentMessage = group.messages.First().Value;
        }
        else
        {
            currentMessage = group.NextMessage(currentMessage);
        }

        if (currentMessage.message == null)
        {
            goto End;
        }
        Choice:
        //messages sent from the player start with an underscore
        GameObject objectToInstantiate;
        if (currentMessage.message[0] != '_')
        {
            objectToInstantiate = messageFromThemPrefab;
            if (!wasLastClickedOn)
            {
                audioManager.PlaySound("Text Notification", false);
            }
        }
        else
        {
            objectToInstantiate = messageFromYouPrefab;
        }
        Text messageTextComponent = Instantiate(objectToInstantiate, dContent.transform)
            .GetComponent<Text>();
        messageTextComponent.text = currentMessage.message.Replace("_", "  ");

        if (currentMessage.choices != null)
        {
            GameObject optionsBox = dContent.transform.parent.parent
                .GetComponentsInChildren<Transform>()
                .FirstOrDefault(x => x.gameObject.name == "Middle")
                .gameObject;

            RectTransform rectTransform = (RectTransform)optionsBox.transform;
            rectTransform.sizeDelta = new Vector2(
                rectTransform.sizeDelta.x,
                15 * currentMessage.choices.Length
            );

            for (int i = 0; i < currentMessage.choices.Length; i++)
            {
                Choice choice = currentMessage.choices[i];
                Text optionTextComponent = Instantiate(optionPrefab, optionsBox.transform)
                    .GetComponent<Text>();
                optionTextComponent.text = choice.choiceMessage;

                int j = i;
                Button optionTextButton = optionTextComponent.gameObject.GetComponent<Button>();
                optionTextButton.onClick.AddListener(
                    delegate
                    {
                        currentMessage = group.NextMessage(currentMessage, j);
                        Text messageFromYouTextComponent = Instantiate(
                                messageFromYouPrefab,
                                dContent.transform
                            )
                            .GetComponent<Text>();
                        messageFromYouTextComponent.text = choice.choiceMessage + "  ";
                        foreach (Transform child in optionsBox.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                        wasLastClickedOn = true;
                    }
                );

                Canvas.ForceUpdateCanvases();
            }
            yield return new WaitWhile(
                delegate
                {
                    return optionsBox.GetComponentsInChildren<Transform>().Length > 1;
                }
            );
            yield return new WaitForSeconds(2f);
            Canvas.ForceUpdateCanvases();
            goto Choice;
        }
        else
        {
            yield return new WaitForSeconds(2f);
            goto NextMessage;
        }
        End:
        chatSelectables[group.from].status.text = "Offline";
        chatSelectables[group.from].status.color = Color.red;
        yield return null;
    }

    public void SendMessageImmediate(GameObject dialogWindow, bool fromYou, string message)
    {
        GameObject objectToInstantiate = fromYou ? messageFromYouPrefab : messageFromThemPrefab;

        Text messageTextComponent = Instantiate(
                objectToInstantiate,
                dialogWindow.GetComponentInChildren<VerticalLayoutGroup>().gameObject.transform
            )
            .GetComponent<Text>();
        messageTextComponent.text = message.Replace("_", "  ");
    }
}
