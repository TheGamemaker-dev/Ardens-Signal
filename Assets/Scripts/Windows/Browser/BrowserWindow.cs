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

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        ChangePage(defaultPage);
    }

    void ChangePage(BrowserPage page)
    {
        GameObject pageInstance = Instantiate(page.gameObject, scrollRect.gameObject.transform);
        Sprite pageImage = pageInstance.GetComponent<Image>().sprite;
        content = pageInstance.GetComponent<RectTransform>();
        scrollRect.content = content;

        float pageWidth = pageImage.rect.width;
        float pageHeight = pageImage.rect.height;
        float contentWidth = content.rect.width;
        float contentHeight = content.rect.height;

        float newHeightDelta = (contentWidth * (pageHeight / pageWidth)) - contentHeight;

        content.sizeDelta = new Vector2(0, newHeightDelta);
    }
}
