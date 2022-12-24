using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsDialogueBoxSizeChangeHandler : MonoBehaviour
{
    const float fullHeight = 163f;

    [SerializeField]
    RectTransform dialogueTransform;
    RectTransform rectTransform;
    bool isReady = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        dialogueTransform.sizeDelta = new Vector2(
            dialogueTransform.sizeDelta.x,
            fullHeight - rectTransform.rect.height
        );
        isReady = true;
    }

    void OnRectTransformDimensionsChange()
    {
        if (isReady)
        {
            float rectHeight = rectTransform.rect.height;
            if (rectHeight == 0)
            {
                rectHeight = 15f;
            }
            dialogueTransform.sizeDelta = new Vector2(
                dialogueTransform.sizeDelta.x,
                fullHeight - rectHeight
            );
        }
    }
}
