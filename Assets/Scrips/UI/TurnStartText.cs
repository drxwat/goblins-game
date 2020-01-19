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
        transform.gameObject.SetActive(true);
        if (timeFor == 0f)
        {
            StartCoroutine(FadeIn(displayText));
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
        yield return StartCoroutine(FadeIn(displayText));
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        StartCoroutine(FadeOutColorToAlpha(1f, wrapper));
        yield return StartCoroutine(FadeOutColorToAlpha(0.3f, text));
        transform.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator FadeIn(string displayText)
    {
        text.text = displayText;
        StartCoroutine(FadeInColorToAlpha(0.5f, wrapper, 0.39f));
        yield return StartCoroutine(FadeInColorToAlpha(1f, text));
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
