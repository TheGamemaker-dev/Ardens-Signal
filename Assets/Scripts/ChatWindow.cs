using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour, IPointerDownHandler
{
    public bool wasLastClickedOn = false;

    [SerializeField] GameObject messageFromThemPrefab, messageFromYouPrefab;
    [SerializeField] GameObject optionPrefab;

    bool wasClickedOn = false;
    Dictionary<string, GameObject> dialogueWindows = new Dictionary<string, GameObject>();
    Dictionary<string, ChatSelectable> chatSelectables = new Dictionary<string, ChatSelectable>();

    void OnEnable()
    {
        GameManager.onFlagSet += UpdateSelectables;
    }

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Dialogue Window"))
            {
                dialogueWindows.Add(child.gameObject.name, child.gameObject);
            }
        }
        ChatSelectable[] selectables = FindObjectsOfType<ChatSelectable>();
        foreach (ChatSelectable selectable in selectables)
        {
            chatSelectables.Add(selectable.gameObject.name, selectable);
        }
        chatSelectables["Signal"].gameObject.SetActive(false);
    }

    void UpdateSelectables(string flag)
    {
        switch (flag)
        {
            case "aiDownloaded":
                chatSelectables["Signal"].gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!wasClickedOn && Input.GetMouseButtonDown(0))
        {
            wasLastClickedOn = false;
        }
        wasClickedOn = false;
    }
    public void OnPointerDown(PointerEventData data)
    {
        wasLastClickedOn = true;
        wasClickedOn = true;
    }

    public void StartMessageGroup(MessageGroup group)
    {
        GameObject dWindow = dialogueWindows[group.from];
        GameObject dContent = new GameObject();
        Transform[] children = dWindow.GetComponentsInChildren<Transform>();
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
        StartCoroutine(SendNextMessage(group, dContent));
    }

    IEnumerator SendNextMessage(MessageGroup group, GameObject dContent)
    {
        //if no choices, wait then send next message
        //if choice, stop send messages and wait for input, then wait and send next message
        //if end, stop
        Message currentMessage = null;
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
        GameObject objectToInstantiate;
        if (currentMessage.message[0] != '_')
        {
            objectToInstantiate = messageFromThemPrefab;
        }
        else
        {
            objectToInstantiate = messageFromYouPrefab;
        }
        Text messageTextComponent = Instantiate(objectToInstantiate, dContent.transform).GetComponent<Text>();
        messageTextComponent.text = currentMessage.message.Replace("_", "") + "  ";
        if (currentMessage.choices != null)
        {
            GameObject optionsBox = dContent.transform.parent.parent.GetComponentsInChildren<Transform>().FirstOrDefault(x => x.gameObject.name == "Middle").gameObject;
            RectTransform rectTransform = (RectTransform)optionsBox.transform;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 15 * currentMessage.choices.Length);
            for (int i = 0; i < currentMessage.choices.Length; i++)
            {
                Choice choice = currentMessage.choices[i];
                Text optionTextComponent = Instantiate(optionPrefab, optionsBox.transform).GetComponent<Text>();
                optionTextComponent.text = choice.choiceMessage;

                int j = i;
                Button optionTextButton = optionTextComponent.gameObject.GetComponent<Button>();
                optionTextButton.onClick.AddListener(delegate
                {
                    currentMessage = group.NextMessage(currentMessage, j);
                    Text messageFromYouTextComponent = Instantiate(messageFromYouPrefab, dContent.transform).GetComponent<Text>();
                    messageFromYouTextComponent.text = choice.choiceMessage + "  ";
                    foreach (Transform child in optionsBox.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                    wasLastClickedOn = true;
                });

                Canvas.ForceUpdateCanvases();
            }
            yield return new WaitWhile(delegate { return optionsBox.GetComponentsInChildren<Transform>().Length > 1; });
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
}
