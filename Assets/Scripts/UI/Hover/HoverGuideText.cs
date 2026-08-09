using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class HoverTMPFade : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("Target Text")]
    [SerializeField] private TMP_Text targetText;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeCoroutine;


    private void Start()
    {
        if (targetText == null)
            return;

        Color color = targetText.color;
        color.a = 0f;
        targetText.color = color;
    }


    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        FadeTo(1f);
    }


    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        FadeTo(0f);
    }


    private void FadeTo(float targetAlpha)
    {
        if (targetText == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine =
            StartCoroutine(
                FadeRoutine(targetAlpha)
            );
    }


    private IEnumerator FadeRoutine(
        float targetAlpha
    )
    {
        Color color =
            targetText.color;

        float startAlpha =
            color.a;

        float time = 0f;


        while (time < fadeDuration)
        {
            time +=
                Time.unscaledDeltaTime;

            float alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    time / fadeDuration
                );

            color =
                targetText.color;

            color.a =
                alpha;

            targetText.color =
                color;

            yield return null;
        }


        color =
            targetText.color;

        color.a =
            targetAlpha;

        targetText.color =
            color;

        fadeCoroutine = null;
    }
}