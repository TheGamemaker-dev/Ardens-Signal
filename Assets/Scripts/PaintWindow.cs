using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintWindow : MonoBehaviour
{
    readonly Color[] colors = { Color.red, new Color32(255, 106, 0, 255), Color.yellow, Color.green, new Color32(0, 121, 255, 255), new Color32(151, 0, 236, 255), new Color32(255, 0, 198, 255), Color.white, Color.gray, Color.black };

    public Color currentColor = Color.black;
    public bool isPaintMode = true;

    [SerializeField] Sprite paintUnselected, paintSelected, eraseUnselected, eraseSelected;
    [SerializeField] GameObject[] buttons = new GameObject[12];


    // Start is called before the first frame update
    void Start()
    {
        SetIsPaintMode(true);
        SelectColor(9);
    }

    public void SetIsPaintMode(bool mode)
    {
        Image paintButton = buttons[10].GetComponent<Image>();
        Image eraserButton = buttons[11].GetComponent<Image>();

        paintButton.sprite = mode ? paintSelected : paintUnselected;
        eraserButton.sprite = mode ? eraseUnselected : eraseSelected;
        isPaintMode = mode;
    }

    public void SelectColor(int colorId)
    {
        if (Mathf.Clamp(colorId, 0, 9) != colorId)
        {
            Debug.LogError("Invalid colorId: " + colorId);
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            if (i == colorId)
            {
                buttons[i].transform.localEulerAngles = new Vector3(180, 0, 0);
            }
            else
            {
                buttons[i].transform.localEulerAngles = Vector3.zero;
            }
        }
        currentColor = colors[colorId];
    }
}
