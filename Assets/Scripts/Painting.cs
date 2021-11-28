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

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        rawImage = GetComponent<RawImage>();
        rectTransform = transform as RectTransform;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        float scale = canvas.scaleFactor;

        texture2D = new Texture2D(Mathf.RoundToInt(scale * width), Mathf.RoundToInt(scale * height));
        rawImage.texture = texture2D;
    }

    void Paint(Vector2Int pos, Color color, int radius)
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < radius * radius; i++)
        {
            colors.Add(color);
        }
        texture2D.SetPixels(pos.x, pos.y, radius, radius, colors.ToArray());
        texture2D.Apply();
    }

    public void OnPointerDown(PointerEventData data)
    {
        Vector2 imagePos = rectTransform.position;
        Vector2 pos = data.position - imagePos;

        Vector2Int posInt = pos.Round();

        Debug.Log(pos);

        Paint(posInt, Color.black, 15);
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 imagePos = rectTransform.position;
        Vector2 pos = data.position - imagePos;

        Vector2 delta = data.delta;
        Vector2 lastPos = pos - delta;

        int signH = Mathf.RoundToInt(Mathf.Sign(delta.x));
        int signV = Mathf.RoundToInt(Mathf.Sign(delta.y));

        float slope;
        slope = delta.y / delta.x;

        if (slope == float.PositiveInfinity)
        {
            slope = Mathf.PI;
        }

        float stepX;
        if (Mathf.Abs(slope) <= 1)
        {
            stepX = 1 * Mathf.Sign(slope); //step by x
        }
        else if (slope == Mathf.PI)
        {
            while (Math.Sign(pos.y - lastPos.y) == signV)
            {
                Paint(lastPos.Round(), Color.black, 15);
                lastPos = lastPos + Vector2.up;
            }
            return;
        }
        else
        {
            stepX = 1 / slope; //step by y
        }

        float stepY = stepX * slope;
        Vector2 step = new Vector2(stepX, stepY);
        while (Mathf.Sign(pos.x - lastPos.x) == signH)
        {
            Paint(lastPos.Round(), Color.black, 15);
            lastPos += step;
        }
    }
}
