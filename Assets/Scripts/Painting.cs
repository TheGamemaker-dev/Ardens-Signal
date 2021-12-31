using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class Painting : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    Texture2D texture2D;
    RawImage rawImage;
    Canvas canvas;
    RectTransform rectTransform;
    bool firstFrame = true;
    Vector2 lastMousePos, currentMousePos;
    float paintScaleFactor = 4f;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        rawImage = GetComponent<RawImage>();
        rectTransform = transform as RectTransform;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        float scale = canvas.scaleFactor;
        scale /= paintScaleFactor;

        texture2D = new Texture2D(Mathf.RoundToInt(scale * width), Mathf.RoundToInt(scale * height));
        rawImage.texture = texture2D;

        List<Color> initialColors = new List<Color>();
        for (int i = 0; i < texture2D.width * texture2D.height; i++)
        {
            initialColors.Add(Color.white);
        }
        texture2D.SetPixels(0, 0, texture2D.width, texture2D.height, initialColors.ToArray());
        texture2D.Apply();
    }

    void Paint(Vector2Int pos, Color color, int radius)
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < radius * radius; i++)
        {
            colors.Add(color);
        }
        texture2D.SetPixels(pos.x - (radius / 2), pos.y - (radius / 2), radius, radius, colors.ToArray());
        texture2D.Apply();
    }

    void Update()
    {
        if (firstFrame)
        {
            currentMousePos = Input.mousePosition;
            firstFrame = false;
        }
        else
        {
            lastMousePos = currentMousePos;
            currentMousePos = Input.mousePosition;
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        Vector2 imagePos = rectTransform.position;
        Vector2 pos = data.position - imagePos;
        pos /= paintScaleFactor;

        Vector2Int posInt = pos.Round();

        Paint(posInt, Color.black, 2);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 imagePos = rectTransform.position;
        Vector2 pos = data.position - imagePos;
        pos /= paintScaleFactor;

        Vector2 delta = data.delta / paintScaleFactor;
        Vector2 lastPos = pos - delta;

        int size = 5;

        int steps = Mathf.RoundToInt(Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)));

        for (int i = 0; i < steps; i++)
        {
            float t = (1f / steps) * i;
            Vector2 drawPos = Vector2.Lerp(lastPos, pos, t);
            Paint(drawPos.Round(), Color.black, size);
        }

        return;
    }
}
