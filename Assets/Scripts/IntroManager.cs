using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image blackImage;
    [SerializeField] private TMP_Text introText;

    [Header("Intro")]
    [TextArea]
    [SerializeField] private List<string> introTexts = new();
    [SerializeField] private float textDuration = 1f; // 텍스트가 다 출력된 후 유지되는 시간
    [SerializeField] private float typingSpeed = 0.05f; // 타이핑 속도

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    private bool isSkipping = false;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    /// <summary>
    /// 전체 인트로 진행
    /// </summary>
    private IEnumerator IntroSequence()
    {
        // 시작 시 검정 -> 투명
        yield return FadeOut();

        // 텍스트 순차 출력
        yield return PlayIntroTexts();

        // 마지막에 검정으로 페이드
        yield return FadeIn();

        SceneManager.LoadScene(nextSceneName);
    }

    /// <summary>
    /// 인트로 텍스트 출력
    /// </summary>
    private IEnumerator PlayIntroTexts()
    {
        introText.gameObject.SetActive(true);

        foreach (string text in introTexts)
        {
            if (isSkipping)
                yield break;

            // 타이핑
            yield return StartCoroutine(TypeText(text));

            // 다 출력된 후 잠깐 유지
            yield return new WaitForSeconds(textDuration);
        }

        introText.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        introText.text = "";

        foreach (char c in text)
        {
            if (isSkipping)
                yield break;

            introText.text += c;

            float delay = typingSpeed;

            //문장부호가 나오면 타이핑 속도를 늦춤
            if (c == ',' || c == '.')
                delay *= 4f;
            else if (c == '!' || c == '?')
                delay *= 5f;

            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// 검정으로 페이드인
    /// </summary>
    public IEnumerator FadeIn()
    {
        float t = 0f;
        Color color = blackImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            blackImage.color = color;
            yield return null;
        }

        color.a = 1;
        blackImage.color = color;
    }

    /// <summary>
    /// 검정에서 화면으로 페이드아웃
    /// </summary>
    public IEnumerator FadeOut()
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

    /// <summary>
    /// Skip 버튼에서 호출
    /// </summary>
    public void SkipIntro()
    {
        if (isSkipping)
            return;

        isSkipping = true;
        StopAllCoroutines();
        StartCoroutine(SkipSequence());
    }

    private IEnumerator SkipSequence()
    {
        introText.gameObject.SetActive(false);

        yield return FadeIn();

        SceneManager.LoadScene(nextSceneName);
    }
}