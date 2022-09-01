using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserWindow : MonoBehaviour
{
    [SerializeField]
    BrowserPage defaultPage;

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

    public void ChangePage(BrowserPage page)
    {
        GameObject pageInstance = Instantiate(page.gameObject, scrollRect.gameObject.transform);
        Sprite pageImage = pageInstance.GetComponent<Image>().sprite;
        if (content != null)
        {
            Destroy(content.gameObject);
        }
        content = pageInstance.GetComponent<RectTransform>();
        scrollRect.content = content;

        float pageWidth = pageImage.rect.width;
        float pageHeight = pageImage.rect.height;
        float contentWidth = content.rect.width;
        float contentHeight = content.rect.height;

        float newHeightDelta = (contentWidth * (pageHeight / pageWidth)) - contentHeight;

        content.sizeDelta = new Vector2(0, newHeightDelta);

        history.Add(page);
    }

    public void GoBack()
    {
        if (history.Count == 0)
        {
            return;
        }

        ChangePage(history[history.Count - 1]);

        history.RemoveRange(history.Count - 2, 2);
    }
}
