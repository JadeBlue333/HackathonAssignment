using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image blackImage;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(ImageFadeOut());
    }


    /// <summary>
    /// 검정에서 화면으로 페이드아웃
    /// </summary>
    public IEnumerator ImageFadeOut()
    {
        float t = 0f;
        Color color = blackImage.color;
        color.a = 1;
        blackImage.color = color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            blackImage.color = color;
            yield return null;
        }

        color.a = 0;
        blackImage.color = color;
    }
}