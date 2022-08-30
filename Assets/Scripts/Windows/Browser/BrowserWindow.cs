using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserWindow : MonoBehaviour
{
    [SerializeField]
    RectTransform content;

    [SerializeField]
    Image contentViewer;

    [SerializeField]
    Sprite defaultPage;

    // Start is called before the first frame update
    void Start()
    {
        ChangePage(defaultPage);
        content.offsetMin = new Vector2(0, -content.rect.height);
    }

    // Update is called once per frame
    void Update() { }

    void ChangePage(Sprite page)
    {
        float pageWidth = page.rect.width;
        float pageHeight = page.rect.height;
        float contentWidth = content.rect.width;

        float newHeight = contentWidth * (pageHeight / pageWidth);

        content.offsetMin = new Vector2(0, -newHeight);
        contentViewer.sprite = page;
    }
}
