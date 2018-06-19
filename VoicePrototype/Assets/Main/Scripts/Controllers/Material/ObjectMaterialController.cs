using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaterialController : MonoBehaviour {
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private float _fadeDuration = 5f;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        DoFadeOut();
    }

    public void DoFadeOut() {
        StartCoroutine(FadeOut(_fadeDuration));
    }

    IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0;
        float targetAlpha = 0;
        float startAlpha = 0.8f;
        Color color = _renderer.material.color;

        while (elapsedTime < duration)
        {
            color.a =  Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            _renderer.material.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
