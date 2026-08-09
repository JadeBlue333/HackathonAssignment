using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CeoDialogue2 : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue")]
    [SerializeField] private List<string> lines = new();

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("GameOverPanel 안에 있는 검정 Image")]
    [SerializeField] private Image blackImage;

    [Tooltip("검정 이미지가 완전히 나타나는 데 걸리는 시간")]
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLine = 0;

    private bool isTyping = false;
    private bool dialogueFinished = false;
    private bool gameOverStarted = false;

    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;


    private void Start()
    {
        // 게임 오버 패널 숨기기
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 검정 이미지 초기화
        if (blackImage != null)
        {
            Color color = blackImage.color;
            color.a = 0f;
            blackImage.color = color;
        }

        // 첫 대사 출력
        if (lines.Count > 0)
        {
            ShowLine();
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            HandleSpace();
        }
    }


    private void HandleSpace()
    {
        // 타이핑 중이면 → 타이핑 즉시 완료
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        // 게임 오버 연출이 시작됐으면 입력 무시
        if (gameOverStarted)
            return;

        // 아직 일반 대사가 남아있으면 → 다음 대사
        if (!dialogueFinished)
        {
            if (currentLine < lines.Count - 1)
            {
                currentLine++;
                ShowLine();
            }
            else
            {
                // 마지막 대사까지 출력 완료
                dialogueFinished = true;

                StartGameOver();
            }
        }
    }


    private void ShowLine()
    {
        if (dialogueText == null)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(lines[currentLine]));
    }


    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.maxVisibleCharacters = i + 1;

            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.maxVisibleCharacters = text.Length;

        isTyping = false;
        typingCoroutine = null;
    }


    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
            dialogueText.maxVisibleCharacters = dialogueText.text.Length;

        isTyping = false;
    }


    // =====================================================
    // Game Over
    // =====================================================

    private void StartGameOver()
    {
        if (gameOverStarted)
            return;

        gameOverStarted = true;

        // 게임 오버 패널 활성화
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // 검정 이미지 페이드인 시작
        if (blackImage != null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeInBlack());
        }
    }


    private IEnumerator FadeInBlack()
    {
        Color color = blackImage.color;
        color.a = 0f;
        blackImage.color = color;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            color.a = alpha;
            blackImage.color = color;

            yield return null;
        }

        // 최종적으로 완전히 검정
        color.a = 1f;
        blackImage.color = color;

        fadeCoroutine = null;
    }


    private void OnDestroy()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }
}