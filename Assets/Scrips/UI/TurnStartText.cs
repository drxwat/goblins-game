using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnStartText : MonoBehaviour
{
    Image wrapper;
    Text text;
    void Start()
    {
        wrapper = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        wrapper.color = new Color(wrapper.color.r, wrapper.color.g, wrapper.color.b, 0f);
    }

    public void ShowText(string displayText, float timeFor = 3f)
    {
        if (timeFor == 0f)
        {
            FadeIn(displayText);
        } else
        {
            StartCoroutine(ShowTemp(displayText, timeFor));
        }
    }

    public void HideText()
    {
        FadeOut();
    }

    IEnumerator ShowTemp(string displayText, float delay = 3f)
    {
        text.text = displayText;
        StartCoroutine(FadeInColorToAlpha(0.5f, wrapper, 0.39f));
        yield return StartCoroutine(FadeInColorToAlpha(1f, text));
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeOutColorToAlpha(1f, wrapper));
        yield return StartCoroutine(FadeOutColorToAlpha(0.3f, text));
    }

    void FadeOut()
    {
        StartCoroutine(FadeOutColorToAlpha(1f, text));
        StartCoroutine(FadeOutColorToAlpha(1f, wrapper));
    }

    void FadeIn(string displayText)
    {
        text.text = displayText;
        StartCoroutine(FadeInColorToAlpha(1f, text));
        StartCoroutine(FadeInColorToAlpha(0.5f, wrapper, 0.39f));
    }

    IEnumerator FadeInColorToAlpha(float time, Graphic text, float alpha = 1f)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a < alpha)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / time));
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeOutColorToAlpha(float time, Graphic text, float alpha = 0f)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a);
        while (text.color.a > alpha)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / time));
            yield return null;
        }
        yield return null;
    }
}
