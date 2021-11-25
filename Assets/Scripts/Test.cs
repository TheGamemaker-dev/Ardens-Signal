using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    Button button;
    Text text;

    MessageGroup messageGroup;
    Message currentMessage;

    public TextAsset testFile;
    public Font font;

    void Start()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();

        messageGroup = MessageGroupCompiler.Compile(testFile);

        button.onClick.AddListener(SetNextMessage);
    }

    void SetNextMessage()
    {
        if (currentMessage == null)
        {
            int firstMessageIndex = messageGroup.lineTypes.FirstOrDefault(x => x.Value == "message").Key;
            currentMessage = messageGroup.messages[firstMessageIndex];
        }
        else
        {
            currentMessage = messageGroup.NextMessage(currentMessage);
        }


        if (currentMessage.choices != null)
        {
            for (int i = 0; i < currentMessage.choices.Length; i++)
            {
                int j = i;

                GameObject choiceButton = Instantiate(new GameObject(), gameObject.transform.parent);
                Button cButton = choiceButton.AddComponent(typeof(Button)) as Button;
                Text cText = choiceButton.AddComponent(typeof(Text)) as Text;

                cButton.onClick.AddListener(delegate
                {
                    currentMessage = messageGroup.NextMessage(currentMessage, j);
                    text.text = currentMessage.message;
                });
                cText.text = currentMessage.choices[i].choiceMessage;
                cText.font = font;
            }
        }
        text.text = currentMessage.message;
    }
}
