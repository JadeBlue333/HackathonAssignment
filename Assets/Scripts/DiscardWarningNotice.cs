using System.Collections;
using UnityEngine;
using TMPro;

public class DiscardWarningNotice : MonoBehaviour
{
    // =========================================================
    // Notice UI
    // =========================================================

    [Header("Notice UI")]

    [SerializeField]
    private GameObject noticeObject;

    [SerializeField]
    private TMP_Text noticeText;

    [Tooltip("발생 후 알림이 나타나기까지의 대기 시간")]
    [SerializeField]
    private float delayBeforeShow = 5f;

    [Tooltip("화면에 떠있는 시간")]
    [SerializeField]
    private float holdDuration = 2.5f;

    [Tooltip("사라지는 시간")]
    [SerializeField]
    private float fadeOutDuration = 0.5f;


    // =========================================================
    // Dialogue
    // =========================================================

    [Header("Dialogue")]

    [TextArea(2, 5)]
    [SerializeField]
    private string[] warningMessages;


    // =========================================================
    // Sound
    // =========================================================

    [Header("Sound")]

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip noticeSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float noticeVolume = 1f;


    // =========================================================
    // Runtime
    // =========================================================

    private CanvasGroup canvasGroup;

    private Coroutine noticeCoroutine;

    private bool isWaitingToShow = false;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (noticeObject == null)
            return;


        canvasGroup =
            noticeObject.GetComponent<CanvasGroup>();


        if (canvasGroup == null)
        {
            canvasGroup =
                noticeObject.AddComponent<CanvasGroup>();
        }


        canvasGroup.alpha = 0f;

        noticeObject.SetActive(false);
    }


    // =========================================================
    // 실패 알림 호출
    // =========================================================

    public void ShowNext()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning(
                "PlayerStatus.Instance가 없습니다."
            );

            return;
        }


        if (
            warningMessages == null ||
            warningMessages.Length == 0
        )
        {
            Debug.LogWarning(
                "폐기 실패 대사가 등록되어 있지 않습니다."
            );

            return;
        }


        // 이미 대기 중이면 중복 예약 방지
        if (isWaitingToShow)
        {
            return;
        }


        isWaitingToShow = true;


        noticeCoroutine =
            StartCoroutine(
                ShowRoutine()
            );
    }


    // =========================================================
    // Notice Routine
    // =========================================================

    private IEnumerator ShowRoutine()
    {
        // -----------------------------------------------------
        // 발생 후 대기
        // -----------------------------------------------------

        if (delayBeforeShow > 0f)
        {
            yield return new WaitForSecondsRealtime(
                delayBeforeShow
            );
        }


        if (
            noticeObject == null ||
            noticeText == null ||
            PlayerStatus.Instance == null
        )
        {
            isWaitingToShow = false;

            noticeCoroutine = null;

            yield break;
        }


        // -----------------------------------------------------
        // 현재 대사 번호 결정
        // -----------------------------------------------------

        int currentIndex =
            PlayerStatus.Instance
                .discardWarningIndex;


        // 마지막 대사를 넘어서면
        // 마지막 인덱스로 고정
        currentIndex =
            Mathf.Clamp(
                currentIndex,
                0,
                warningMessages.Length - 1
            );


        // -----------------------------------------------------
        // 대사 적용
        // -----------------------------------------------------

        noticeText.text =
            warningMessages[currentIndex];


        // -----------------------------------------------------
        // 다음 대사로 진행
        // -----------------------------------------------------

        if (
            PlayerStatus.Instance.discardWarningIndex <
            warningMessages.Length - 1
        )
        {
            PlayerStatus.Instance
                .discardWarningIndex++;
        }


        // 마지막 대사에 도달한 뒤에는
        // 인덱스를 증가시키지 않음
        // → 계속 마지막 대사 반복


        isWaitingToShow = false;


        // -----------------------------------------------------
        // Notice ON
        // -----------------------------------------------------

        noticeObject.SetActive(true);


        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }


        // -----------------------------------------------------
        // Notice Sound
        // -----------------------------------------------------

        if (
            audioSource != null &&
            noticeSound != null
        )
        {
            audioSource.PlayOneShot(
                noticeSound,
                noticeVolume
            );
        }


        // -----------------------------------------------------
        // Hold
        // -----------------------------------------------------

        yield return new WaitForSecondsRealtime(
            holdDuration
        );


        // -----------------------------------------------------
        // Fade Out
        // -----------------------------------------------------

        if (
            canvasGroup != null &&
            fadeOutDuration > 0f
        )
        {
            float elapsed = 0f;


            while (
                elapsed <
                fadeOutDuration
            )
            {
                elapsed +=
                    Time.unscaledDeltaTime;


                float t =
                    Mathf.Clamp01(
                        elapsed /
                        fadeOutDuration
                    );


                canvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );


                yield return null;
            }
        }


        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }


        noticeObject.SetActive(false);


        noticeCoroutine = null;
    }


    // =========================================================
    // Inspector 값 보정
    // =========================================================

    private void OnValidate()
    {
        delayBeforeShow =
            Mathf.Max(
                0f,
                delayBeforeShow
            );

        holdDuration =
            Mathf.Max(
                0f,
                holdDuration
            );

        fadeOutDuration =
            Mathf.Max(
                0f,
                fadeOutDuration
            );
    }
}