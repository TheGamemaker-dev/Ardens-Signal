using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float length;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void FadeIn()
    {
        StartCoroutine(DoFadeIn());
    }

    public void FadeOut()
    {
        StartCoroutine(DoFadeOut());
    }

    IEnumerator DoFadeOut()
    {
        transform.SetAsLastSibling();
        while (image.color.a != 1)
        {
            Color curCol = image.color;
            float newAlpha = Mathf.Min(curCol.a + (Time.deltaTime / length), 1);

            image.color = new Color(curCol.r, curCol.g, curCol.b, newAlpha);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator DoFadeIn()
    {
        transform.SetAsLastSibling();
        while (image.color.a != 0)
        {
            Color curCol = image.color;
            float newAlpha = Mathf.Max(curCol.a - (Time.deltaTime / length), 0);

            image.color = new Color(curCol.r, curCol.g, curCol.b, newAlpha);
            yield return new WaitForEndOfFrame();
        }
        transform.SetAsFirstSibling();
    }
}
