using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image blackImage;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Black Screen")]
    [SerializeField] private float blackHoldTime = 0.5f;

    private void Start()
    {
        // Black Image가 들어있는 GameObject가 비활성화되어 있다면 활성화
        if (!blackImage.gameObject.activeSelf)
        {
            blackImage.gameObject.SetActive(true);
        }

        StartCoroutine(ImageFadeOut());
    }

    /// <summary>
    /// 검은 화면 → 게임 화면
    /// 실제 동작은 페이드 인처럼 보임
    /// </summary>
    public IEnumerator ImageFadeOut()
    {
        float t = 0f;

        Color color = blackImage.color;
        color.a = 1f;
        blackImage.color = color;

        yield return new WaitForSeconds(blackHoldTime);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            blackImage.color = color;

            yield return null;
        }

        color.a = 0f;
        blackImage.color = color;
    }
}