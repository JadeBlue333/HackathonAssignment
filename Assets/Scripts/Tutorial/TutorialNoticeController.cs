using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNoticeController : MonoBehaviour
{
    [Header("Tutorial Notice")]
    [SerializeField]
    private GameObject tutorialNoticeCanvas;

    [SerializeField]
    private TMP_Text countdownText;

    [Tooltip("튜토리얼 씬으로 넘어가기까지의 시간")]
    [SerializeField]
    private int countdownSeconds = 5;

    [Tooltip("점이 바뀌는 속도")]
    [SerializeField]
    private float dotInterval = 0.3f;


    private int currentSeconds;
    private string currentDots = ".";

    private bool isCountingDown = false;


    private void Start()
    {
        if (PlayerStatus.Instance == null)
        {
            tutorialNoticeCanvas.SetActive(false);
            return;
        }

        if (!PlayerStatus.Instance.tutorialCompleted)
        {
            tutorialNoticeCanvas.SetActive(true);

            isCountingDown = true;

            StartCoroutine(CountdownRoutine());
            StartCoroutine(DotAnimationRoutine());
        }
        else
        {
            tutorialNoticeCanvas.SetActive(false);
        }
    }


    // =========================================================
    // Countdown
    // =========================================================

    private IEnumerator CountdownRoutine()
    {
        currentSeconds = countdownSeconds;

        while (currentSeconds > 0)
        {
            UpdateCountdownText();

            yield return new WaitForSeconds(1f);

            currentSeconds--;
        }

        isCountingDown = false;

        SceneManager.LoadScene("Tutorial");
    }


    // =========================================================
    // Dot Animation
    // =========================================================

    private IEnumerator DotAnimationRoutine()
    {
        int dotCount = 1;

        while (isCountingDown)
        {
            currentDots = new string('.', dotCount);

            UpdateCountdownText();

            dotCount++;

            if (dotCount > 3)
            {
                dotCount = 1;
            }

            yield return new WaitForSeconds(dotInterval);
        }
    }


    // =========================================================
    // Text
    // =========================================================

    private void UpdateCountdownText()
    {
        countdownText.text =
            $"{currentSeconds}초 뒤 교육 프로그램이 자동으로 실행됩니다{currentDots}";
    }
}