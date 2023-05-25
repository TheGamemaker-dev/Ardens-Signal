using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class Painting : MonoBehaviour, IDragHandler
{
    Texture2D texture2D;
    RawImage rawImage;
    Canvas canvas;
    RectTransform rectTransform;
    PaintWindow paintWindow;
    bool firstFrame = true;
    Vector2 lastMousePos, currentMousePos;
    float paintScaleFactor = 2f;
    int size = 8;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        rawImage = GetComponent<RawImage>();
        paintWindow = FindObjectOfType<PaintWindow>();
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

    void Paint(Vector2Int pos, Color color, int diameter)
    {
        List<Color> colors = new List<Color>();

        for (int i = 0; i < diameter * diameter; i++)
        {
            int x = (i % diameter) - (diameter / 2);
            int y = (Mathf.FloorToInt(i / diameter)) - (diameter / 2);
            float sqrDistance = new Vector2(x, y).sqrMagnitude;

            if (sqrDistance < Mathf.Pow(diameter / 2, 2))
            {
                colors.Add(color);
            }
            else
            {
                int absoluteX = pos.x + x;
                int absolutey = pos.y + y;

                Color colorToAdd = texture2D.GetPixel(absoluteX, absolutey);
                colors.Add(colorToAdd);
            }
        }
        texture2D.SetPixels(pos.x - (diameter / 2), pos.y - (diameter / 2), diameter, diameter, colors.ToArray());
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

    public void OnDrag(PointerEventData data)
    {
        Vector2 imagePos = rectTransform.position;
        Vector2 pos = data.position - imagePos;
        pos /= paintScaleFactor;

        Vector2 delta = data.delta / paintScaleFactor;
        Vector2 lastPos = pos - delta;


        int steps = Mathf.RoundToInt(Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y)));

        for (int i = 0; i < steps; i++)
        {
            float t = (1f / steps) * i;
            Vector2 drawPos = Vector2.Lerp(lastPos, pos, t);
            Paint(drawPos.Round(), paintWindow.isPaintMode ? paintWindow.currentColor : Color.white, size);
        }

        return;
    }
}
