using System.Collections;
using UnityEngine;

public class InspectionFailEffect : MonoBehaviour
{
    // =========================================================
    // Camera Shake
    // =========================================================

    [Header("Camera Shake")]

    [Tooltip("흔들릴 카메라 또는 Camera Pivot")]
    [SerializeField]
    private Transform cameraTransform;

    [Tooltip("카메라 흔들림 지속 시간")]
    [SerializeField]
    private float shakeDuration = 0.5f;

    [Tooltip("카메라 흔들림 강도")]
    [SerializeField]
    private float shakeStrength = 0.08f;


    // =========================================================
    // Post Processing
    // =========================================================

    [Header("Post Processing")]

    [Tooltip("충격 효과용 Post Process Volume 오브젝트")]
    [SerializeField]
    private GameObject failPostProcess;

    [Tooltip("Post Processing 효과 유지 시간")]
    [SerializeField]
    private float postProcessDuration = 2f;


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


        if (failPostProcess != null)
        {
            failPostProcess.SetActive(
                false
            );
        }
    }


    // =========================================================
    // Play
    // =========================================================

    public void Play()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(
                effectCoroutine
            );
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
        // Post Processing ON
        // -----------------------------------------------------

        if (failPostProcess != null)
        {
            failPostProcess.SetActive(
                true
            );
        }


        // -----------------------------------------------------
        // Camera Shake
        // -----------------------------------------------------

        float elapsed =
            0f;


        while (
            elapsed <
            shakeDuration
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;


            if (cameraTransform != null)
            {
                Vector3 offset =
                    Random.insideUnitSphere *
                    shakeStrength;


                cameraTransform.localPosition =
                    originalCameraLocalPosition +
                    offset;
            }


            yield return null;
        }


        // 카메라 원위치
        if (cameraTransform != null)
        {
            cameraTransform.localPosition =
                originalCameraLocalPosition;
        }


        // -----------------------------------------------------
        // 남은 Post Processing 시간 대기
        // -----------------------------------------------------

        float remainingTime =
            postProcessDuration -
            shakeDuration;


        if (remainingTime > 0f)
        {
            yield return new WaitForSecondsRealtime(
                remainingTime
            );
        }


        // -----------------------------------------------------
        // Post Processing OFF
        // -----------------------------------------------------

        if (failPostProcess != null)
        {
            failPostProcess.SetActive(
                false
            );
        }


        effectCoroutine =
            null;
    }
}