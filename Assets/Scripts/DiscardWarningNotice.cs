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
    // Boot Effect
    // =========================================================

    [Header("Boot Effect")]

    [Tooltip("등장 시 깜빡임 효과 사용")]
    [SerializeField]
    private bool useBootEffect = true;

    [Tooltip("알림 활성화 후 첫 점멸까지 대기 시간")]
    [SerializeField]
    private float bootStartDelay = 0.05f;


    // =========================================================
    // Dialogue - Korean
    // =========================================================

    [Header("Dialogue - Korean")]

    [TextArea(2, 5)]
    [SerializeField]
    private string[] warningMessagesKR;


    // =========================================================
    // Dialogue - English
    // =========================================================

    [Header("Dialogue - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string[] warningMessagesEN;


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


        if (!HasMessages())
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
        // 현재 사용 언어의 대사 배열
        // -----------------------------------------------------

        string[] currentMessages =
            GetCurrentMessages();


        if (
            currentMessages == null ||
            currentMessages.Length == 0
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


        currentIndex =
            Mathf.Clamp(
                currentIndex,
                0,
                currentMessages.Length - 1
            );


        // -----------------------------------------------------
        // 대사 적용
        // -----------------------------------------------------

        noticeText.text =
            currentMessages[currentIndex];


        // -----------------------------------------------------
        // 다음 대사로 진행
        // -----------------------------------------------------

        int maxMessageCount =
            GetMaxMessageCount();


        if (
            PlayerStatus.Instance.discardWarningIndex <
            maxMessageCount - 1
        )
        {
            PlayerStatus.Instance
                .discardWarningIndex++;
        }


        isWaitingToShow = false;


        // -----------------------------------------------------
        // Notice ON
        // -----------------------------------------------------

        noticeObject.SetActive(true);


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
        // Boot Effect
        // -----------------------------------------------------

        if (
            useBootEffect &&
            canvasGroup != null
        )
        {
            yield return StartCoroutine(
                BootEffectRoutine()
            );
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
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
    // Current Language Messages
    // =========================================================

    private string[] GetCurrentMessages()
    {
        if (
            LanguageManager.Instance != null &&
            LanguageManager.Instance.isEnglish
        )
        {
            return warningMessagesEN;
        }


        return warningMessagesKR;
    }


    // =========================================================
    // Has Messages
    // =========================================================

    private bool HasMessages()
    {
        string[] currentMessages =
            GetCurrentMessages();


        return
            currentMessages != null &&
            currentMessages.Length > 0;
    }


    // =========================================================
    // Max Message Count
    // =========================================================

    private int GetMaxMessageCount()
    {
        int koreanCount =
            warningMessagesKR != null
                ? warningMessagesKR.Length
                : 0;


        int englishCount =
            warningMessagesEN != null
                ? warningMessagesEN.Length
                : 0;


        // 두 언어 중 더 짧은 배열을 기준으로 사용
        // KR / EN 대사 번호가 어긋나는 것을 방지
        return Mathf.Min(
            koreanCount,
            englishCount
        );
    }


    // =========================================================
    // Boot Effect Routine
    // =========================================================

    private IEnumerator BootEffectRoutine()
    {
        canvasGroup.alpha = 0f;


        if (bootStartDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                bootStartDelay
            );
        }


        // 첫 번째 점멸
        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(
            0.05f
        );


        canvasGroup.alpha = 0f;

        yield return new WaitForSecondsRealtime(
            0.08f
        );


        // 두 번째 점멸
        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(
            0.03f
        );


        canvasGroup.alpha = 0f;

        yield return new WaitForSecondsRealtime(
            0.05f
        );


        // 세 번째 점멸
        canvasGroup.alpha = 0.5f;

        yield return new WaitForSecondsRealtime(
            0.04f
        );


        // 최종 고정
        canvasGroup.alpha = 1f;
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

        bootStartDelay =
            Mathf.Max(
                0f,
                bootStartDelay
            );
    }
}