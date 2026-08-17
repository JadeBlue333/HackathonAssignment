using System.Collections;
using UnityEngine;

public class DiscardWarningNotice : MonoBehaviour
{
    // =========================================================
    // Notice UI
    // =========================================================

    [Header("Notice UI")]

    [Tooltip("처음 1회만 표시할 경고 이미지")]
    [SerializeField]
    private GameObject noticeObject;

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
    // 최초 1회 알림
    // =========================================================

    public void ShowOnce()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning(
                "PlayerStatus.Instance가 없습니다."
            );

            return;
        }


        // 이미 실제로 본 적 있으면 표시하지 않음
        if (
            PlayerStatus.Instance
                .hasSeenDiscardWarning
        )
        {
            return;
        }


        // 이미 5초 대기 중이라면
        // 또 예약하지 않음
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


        if (noticeObject == null)
        {
            isWaitingToShow = false;
            noticeCoroutine = null;

            yield break;
        }


        if (PlayerStatus.Instance == null)
        {
            isWaitingToShow = false;
            noticeCoroutine = null;

            yield break;
        }


        // -----------------------------------------------------
        // 여기까지 왔으면 실제로 알림을 보여줌
        // 이때 "본 적 있음"으로 기록
        // -----------------------------------------------------

        PlayerStatus.Instance
            .hasSeenDiscardWarning = true;


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