using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class InspectionFailEffect : MonoBehaviour
{
    // =========================================================
    // Camera Shake
    // =========================================================

    [Header("Camera Shake")]

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float shakeDuration = 0.35f;

    [SerializeField]
    private float shakeStrength = 0.06f;


    // =========================================================
    // Post Processing
    // =========================================================

    [Header("Post Processing")]

    [SerializeField]
    private Volume failVolume;

    [Tooltip("효과가 강하게 올라오는 시간")]
    [SerializeField]
    private float postProcessFadeIn = 0.08f;

    [Tooltip("최대 효과를 유지하는 시간")]
    [SerializeField]
    private float postProcessHold = 0.35f;

    [Tooltip("효과가 자연스럽게 사라지는 시간")]
    [SerializeField]
    private float postProcessFadeOut = 1.2f;


    // =========================================================
    // Sound
    // =========================================================

    [Header("Sound")]

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip failSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float failSoundVolume = 1f;


    // =========================================================
    // Runtime
    // =========================================================

    private Vector3 originalCameraLocalPosition;

    private Coroutine effectCoroutine;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (cameraTransform != null)
        {
            originalCameraLocalPosition =
                cameraTransform.localPosition;
        }

        if (failVolume != null)
        {
            failVolume.weight = 0f;
        }
    }


    // =========================================================
    // Play
    // =========================================================

    public void Play()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);

            if (cameraTransform != null)
            {
                cameraTransform.localPosition =
                    originalCameraLocalPosition;
            }

            if (failVolume != null)
            {
                failVolume.weight = 0f;
            }
        }

        effectCoroutine =
            StartCoroutine(
                EffectRoutine()
            );
    }


    // =========================================================
    // Effect
    // =========================================================

    private IEnumerator EffectRoutine()
    {
        // -----------------------------------------------------
        // Sound
        // -----------------------------------------------------

        if (
            audioSource != null &&
            failSound != null
        )
        {
            audioSource.PlayOneShot(
                failSound,
                failSoundVolume
            );
        }


        // -----------------------------------------------------
        // Post Processing Fade In
        // -----------------------------------------------------

        if (failVolume != null)
        {
            float elapsed = 0f;

            while (elapsed < postProcessFadeIn)
            {
                elapsed +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        postProcessFadeIn
                    );

                failVolume.weight =
                    Mathf.Lerp(
                        0f,
                        1f,
                        t
                    );

                yield return null;
            }

            failVolume.weight = 1f;
        }


        // -----------------------------------------------------
        // Camera Shake
        // -----------------------------------------------------

        float shakeElapsed = 0f;

        while (shakeElapsed < shakeDuration)
        {
            shakeElapsed +=
                Time.unscaledDeltaTime;

            if (cameraTransform != null)
            {
                float normalized =
                    1f -
                    Mathf.Clamp01(
                        shakeElapsed /
                        shakeDuration
                    );

                float currentStrength =
                    shakeStrength *
                    normalized;

                Vector2 offset =
                    Random.insideUnitCircle *
                    currentStrength;

                cameraTransform.localPosition =
                    originalCameraLocalPosition +
                    new Vector3(
                        offset.x,
                        offset.y,
                        0f
                    );
            }

            yield return null;
        }


        // -----------------------------------------------------
        // Camera Restore
        // -----------------------------------------------------

        if (cameraTransform != null)
        {
            cameraTransform.localPosition =
                originalCameraLocalPosition;
        }


        // -----------------------------------------------------
        // Hold
        // -----------------------------------------------------

        if (postProcessHold > 0f)
        {
            yield return new WaitForSecondsRealtime(
                postProcessHold
            );
        }


        // -----------------------------------------------------
        // Post Processing Fade Out
        // -----------------------------------------------------

        if (failVolume != null)
        {
            float elapsed = 0f;

            while (elapsed < postProcessFadeOut)
            {
                elapsed +=
                    Time.unscaledDeltaTime;

                float t =
                    Mathf.Clamp01(
                        elapsed /
                        postProcessFadeOut
                    );

                // 처음엔 빨리, 끝으로 갈수록 천천히 복구
                float easedT =
                    1f -
                    Mathf.Pow(
                        1f - t,
                        2f
                    );

                failVolume.weight =
                    Mathf.Lerp(
                        1f,
                        0f,
                        easedT
                    );

                yield return null;
            }

            failVolume.weight = 0f;
        }


        effectCoroutine =
            null;
    }


    // =========================================================
    // Disable
    // =========================================================

    private void OnDisable()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localPosition =
                originalCameraLocalPosition;
        }

        if (failVolume != null)
        {
            failVolume.weight = 0f;
        }
    }
}