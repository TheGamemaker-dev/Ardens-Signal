using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserWindow : MonoBehaviour
{
    [SerializeField]
    BrowserPage defaultPage;

    [SerializeField]
    Button backButton,
        forwardButton;

    [SerializeField]
    Text urlField;

    ScrollRect scrollRect;
    RectTransform content;
    List<BrowserPage> history = new List<BrowserPage>();
    List<BrowserPage> future = new List<BrowserPage>();

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        ChangePage(defaultPage);
    }

    private void Update()
    {
        backButton.interactable = history.Count > 0;
        forwardButton.interactable = future.Count > 0;
    }

    public void ChangePage(BrowserPage page)
    {
        GameObject pageInstance = Instantiate(page.gameObject, scrollRect.gameObject.transform);
        Sprite pageImage = pageInstance.GetComponent<Image>().sprite;
        if (content != null)
        {
            history.Add(content.gameObject.GetComponent<BrowserPage>());
            content.gameObject.SetActive(false);
        }
        ChangeContent(pageInstance.GetComponent<BrowserPage>());

        float pageWidth = pageImage.rect.width;
        float pageHeight = pageImage.rect.height;
        float contentWidth = content.rect.width;
        float contentHeight = content.rect.height;

        float newHeightDelta = (contentWidth * (pageHeight / pageWidth)) - contentHeight;

        content.sizeDelta = new Vector2(0, newHeightDelta);
        future.Clear();
    }

    public void GoBack()
    {
        if (history.Count == 0)
        {
            return;
        }

        future.Add(content.gameObject.GetComponent<BrowserPage>());
        content.gameObject.SetActive(false);

        int histIndex = history.Count - 1;
        history[histIndex].gameObject.SetActive(true);
        ChangeContent(history[histIndex]);
        history.RemoveAt(histIndex);
    }

    public void GoForward()
    {
        if (future.Count == 0)
        {
            return;
        }

        history.Add(content.gameObject.GetComponent<BrowserPage>());
        content.gameObject.SetActive(false);

        int futureIndex = future.Count - 1;
        future[futureIndex].gameObject.SetActive(true);
        ChangeContent(future[futureIndex]);
        future.RemoveAt(futureIndex);
    }

    void ChangeContent(BrowserPage newContent)
    {
        content = newContent.gameObject.GetComponent<RectTransform>();
        scrollRect.content = content;
        urlField.text = "https://www." + newContent.url;
    }
}
