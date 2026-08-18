using System.Collections;
using TMPro;
using UnityEngine;

public class HintNotice : MonoBehaviour
{
    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [SerializeField]
    private GameObject noticeObject;

    [SerializeField]
    private TMP_Text noticeText;

    [SerializeField]
    private CanvasGroup canvasGroup;


    // =========================================================
    // Timing
    // =========================================================

    [Header("Timing")]

    [Tooltip("화면에 유지되는 시간")]
    [SerializeField]
    private float holdDuration = 3f;

    [Tooltip("사라지는 시간")]
    [SerializeField]
    private float fadeOutDuration = 0.5f;


    // =========================================================
    // Boot Effect
    // =========================================================

    [Header("Boot Effect")]

    [SerializeField]
    private bool useBootEffect = true;

    [SerializeField]
    private float bootStartDelay = 0.05f;


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

    private Coroutine noticeCoroutine;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (noticeObject != null)
        {
            noticeObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }


    // =========================================================
    // Show
    // =========================================================

    public void Show(string message)
    {
        if (noticeObject == null ||
            noticeText == null)
        {
            return;
        }


        if (noticeCoroutine != null)
        {
            StopCoroutine(noticeCoroutine);
        }


        noticeText.text =
            message;


        noticeObject.SetActive(
            true
        );


        noticeCoroutine =
            StartCoroutine(
                ShowRoutine()
            );
    }


    // =========================================================
    // Show Routine
    // =========================================================

    private IEnumerator ShowRoutine()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }


        // -----------------------------------------------------
        // Sound
        // -----------------------------------------------------

        if (audioSource != null &&
            noticeSound != null)
        {
            audioSource.PlayOneShot(
                noticeSound,
                noticeVolume
            );
        }


        // -----------------------------------------------------
        // Boot Effect
        // -----------------------------------------------------

        if (useBootEffect &&
            canvasGroup != null)
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

        if (holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                holdDuration
            );
        }


        // -----------------------------------------------------
        // Fade Out
        // -----------------------------------------------------

        if (canvasGroup != null &&
            fadeOutDuration > 0f)
        {
            float elapsed = 0f;


            while (elapsed < fadeOutDuration)
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


        noticeObject.SetActive(
            false
        );


        noticeCoroutine = null;
    }


    // =========================================================
    // Boot Effect
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


        // 최종 표시
        canvasGroup.alpha = 1f;
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
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