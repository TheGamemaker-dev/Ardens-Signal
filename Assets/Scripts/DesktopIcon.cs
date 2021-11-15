using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(DoubleClickHandler))]
public class DesktopIcon : MonoBehaviour, IPointerDownHandler
{
    Image image, textContainer;
    Text label;
    DoubleClickHandler doubleClickHandler;

    bool isHighlighted = false;
    bool clickedOn = false;
    Color highlightedColor = new Color32(74, 58, 253, 255);

    [SerializeField] Sprite icon;
    [SerializeField] string labelText;
    [SerializeField] GameObject windowPrefab;

    #region Setup
    void Awake()
    {
        doubleClickHandler = GetComponent<DoubleClickHandler>();

        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.gameObject.name == "Icon")
            {
                image = img;
            }
            else
            {
                textContainer = img;
            }
        }

        label = GetComponentInChildren<Text>();
    }
    void OnEnable()
    {
        doubleClickHandler.doubleClicked += OpenWindow;
    }
    void Start()
    {
        image.sprite = icon;
        image.color = Color.white;
        textContainer.color = new Color(0, 0, 0, 0);
        label.text = labelText;
    }
    void OnDisable()
    {
        doubleClickHandler.doubleClicked -= OpenWindow;
    }
    #endregion

    #region Behavior
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !clickedOn)
        {
            SetHighlighted(false);
        }
        clickedOn = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        clickedOn = true;
        SetHighlighted(true);
    }

    void SetHighlighted(bool highlighted)
    {
        if (highlighted)
        {
            image.color = highlightedColor;
            textContainer.color = highlightedColor;
        }
        else
        {
            image.color = Color.white;
            textContainer.color = new Color(0, 0, 0, 0);
        }
        isHighlighted = highlighted;
    }

    void OpenWindow()
    {
        Debug.Log("Open window");
    }
    #endregion
}
