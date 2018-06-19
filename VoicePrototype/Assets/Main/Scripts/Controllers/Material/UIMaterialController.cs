using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMaterialController : MonoBehaviour {
    [SerializeField]
    protected float fadeDuration = 1.5f;
    [SerializeField]
    protected float scaleMultiplier = 1.2f;

    public abstract bool HasMaterialColor { get; }
    public abstract Color MaterialColor { get; set; }

    public virtual void DoHighlight()
    {
        transform.localScale = transform.localScale * scaleMultiplier;
    }

    public virtual void DoUnhighlight()
    {
        transform.localScale = transform.localScale / scaleMultiplier;
    }

    public void DoBlinkandFadeOut()
    {
        StartCoroutine(BlinkandFadeOut(fadeDuration, transform.localScale * scaleMultiplier));
    }

    public void DoFadeOut()
    {
        StartCoroutine(FadeOut(fadeDuration, transform.localScale));
    }

    public void DoFadeIn()
    {
        transform.gameObject.SetActive(true);

        StartCoroutine(FadeIn(fadeDuration));
    }

    //make timing customisable
    IEnumerator BlinkandFadeOut(float duration, Vector3 targetScale)
    {
        Color opaqueColor = MaterialColor;
        Color transparentColor = MaterialColor;
        transparentColor.a = 0;

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSecondsRealtime(0.15f);
            MaterialColor = transparentColor;
            yield return new WaitForSecondsRealtime(0.15f);
            MaterialColor = opaqueColor;
        }

        float elapsedTime = 0;

        float startAlpha = 1;
        float targetAlpha = 0;
        Color color = MaterialColor;

        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            MaterialColor = color;

            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.gameObject.SetActive(false);
    }

    IEnumerator FadeOut(float duration, Vector3 targetScale)
    {
        float elapsedTime = 0;

        float startAlpha = 1;
        float targetAlpha = 0;
        Color color = MaterialColor;

        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            MaterialColor = color;

            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.gameObject.SetActive(false);
    }

    IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0;

        float startAlpha = 0;
        float targetAlpha = 1;
        Color color = MaterialColor;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            MaterialColor = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
