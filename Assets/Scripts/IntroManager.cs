using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
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
    [SerializeField] private float textDuration = 1f;
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("BGM")]
    [SerializeField] private AudioSource bgm;
    [SerializeField] private float bgmFadeDuration = 1f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    [Header("맨 처음 인트로 화면일때 true")]
    public bool startOfGame = false;
    [SerializeField] private GameObject chatImg;

    [Header("엔딩 화면일때 true")]
    public bool endOfGame = false;

    private bool isSpacePressed = false;
    private bool isSkipping = false;

    // 엔딩용 텍스트 색상
    private readonly Color endTextColor = new Color32(0x5D, 0xD6, 0xF2, 0xFF);

    private void Start()
    {
        // 게임이 끝나는 씬이면 PlayerStatus 삭제
        if (endOfGame)
        {
            Destroy(PlayerStatus.Instance.gameObject);
        }

        StartCoroutine(PlayIntroTexts());

        // 엔딩이 아닐 때만 처음부터 BGM 재생
        if (!endOfGame && bgm != null)
        {
            bgm.Play();
        }
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isSpacePressed = true;
        }
    }

    /// <summary>
    /// 전체 인트로 진행
    /// </summary>
    private IEnumerator PlayIntroTexts()
    {
        introText.gameObject.SetActive(true);

        for (int i = 0; i < introTexts.Count; i++)
        {
            if (isSkipping)
                yield break;

            // 처음 게임 시작이고,
            // 마지막에서 두 번째 텍스트일 때 chatImg 활성화
            if (startOfGame && i == introTexts.Count - 2)
            {
                if (chatImg != null)
                    chatImg.SetActive(true);
            }

            // 엔딩일 경우
            if (endOfGame)
            {
                // 첫 번째 문장 → 흰색
                // 두 번째 문장부터 → #5DD6F2
                if (i == 0)
                {
                    introText.color = Color.white;
                }
                else
                {
                    introText.color = endTextColor;

                    // 두 번째 문장이 시작될 때 BGM 재생
                    if (i == 1 && bgm != null)
                    {
                        bgm.Play();
                    }
                }
            }

            isSpacePressed = false;

            // 타이핑
            yield return StartCoroutine(TypeText(introTexts[i]));

            if (isSkipping)
                yield break;

            // 타이핑 중 Space를 눌렀다면
            // 이번 프레임의 입력을 소비
            if (isSpacePressed)
            {
                isSpacePressed = false;
            }

            // 스페이스바를 누를 때까지 대기
            yield return new WaitUntil(() => isSpacePressed);

            isSpacePressed = false;
        }

        introText.gameObject.SetActive(false);

        // 마지막 대사까지 끝났으면 다음 씬
        yield return FadeIn();

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator TypeText(string text)
    {
        introText.text = "";

        foreach (char c in text)
        {
            if (isSkipping)
                yield break;

            // 스페이스를 누르면 타이핑 즉시 완료
            if (isSpacePressed)
            {
                introText.text = text;
                yield break;
            }

            introText.text += c;

            float delay = typingSpeed;

            // 문장부호가 나오면 타이핑 속도를 늦춤
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

        if (bgm == null)
            yield break;

        // BGM 페이드아웃
        float startVolume = bgm.volume;
        float t2 = 0f;

        while (t2 < bgmFadeDuration)
        {
            t2 += Time.deltaTime;

            bgm.volume = Mathf.Lerp(
                startVolume,
                0f,
                t2 / bgmFadeDuration
            );

            yield return null;
        }

        bgm.volume = 0f;
        bgm.Stop();
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