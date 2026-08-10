using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DailyReportBGM : MonoBehaviour
{
    public static DailyReportBGM Instance { get; private set; }


    // =====================================================
    // Audio
    // =====================================================

    [Header("Audio")]

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip bgm;


    // =====================================================
    // Daily Report
    // =====================================================

    [Header("Daily Report")]

    [Tooltip("하루 종료 후 들어가는 DailyReport 씬 이름")]
    [SerializeField]
    private string dailyReportSceneName = "DailyReport";


    // =====================================================
    // Fade
    // =====================================================

    [Header("Fade")]

    [Tooltip("BGM이 시작될 때 페이드인 시간")]
    [SerializeField]
    private float fadeInDuration = 1.5f;

    [Tooltip("BGM이 종료될 때 페이드아웃 시간")]
    [SerializeField]
    private float fadeOutDuration = 1.5f;

    [Range(0f, 1f)]
    [SerializeField]
    private float maxVolume = 1f;


    // =====================================================
    // Runtime
    // =====================================================

    private Coroutine fadeCoroutine;

    private string startSceneName;

    private bool hasEnteredDailyReport = false;

    private bool isEnding = false;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }


        Instance =
            this;


        DontDestroyOnLoad(
            gameObject
        );


        // 이 BGM이 처음 시작된 씬 저장
        startSceneName =
            SceneManager
                .GetActiveScene()
                .name;


        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }


    // =====================================================
    // Start
    // =====================================================

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError(
                "AudioSource가 연결되어 있지 않습니다."
            );

            return;
        }


        if (bgm == null)
        {
            Debug.LogError(
                "BGM AudioClip이 연결되어 있지 않습니다."
            );

            return;
        }


        audioSource.loop =
            true;


        PlayWithFade();
    }


    // =====================================================
    // Scene Loaded
    // =====================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (isEnding)
            return;


        // =================================================
        // 시작 씬
        // =================================================

        if (
            scene.name ==
            startSceneName
        )
        {
            return;
        }


        // =================================================
        // DailyReport 진입
        //
        // 같은 BGM 유지
        // =================================================

        if (
            scene.name ==
            dailyReportSceneName
        )
        {
            hasEnteredDailyReport =
                true;

            return;
        }


        // =================================================
        // 시작 씬도 아니고 DailyReport도 아닌 씬 진입
        //
        // DailyReport를 거쳤든,
        // 엔딩 조건으로 바로 다른 씬으로 갔든
        // 여기서 BGM 종료
        // =================================================

        StartFadeOutAndDestroy();
    }


    // =====================================================
    // Play
    // =====================================================

    private void PlayWithFade()
    {
        if (
            audioSource == null ||
            bgm == null
        )
        {
            return;
        }


        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }


        fadeCoroutine =
            StartCoroutine(
                FadeInRoutine()
            );
    }


    // =====================================================
    // Fade In
    // =====================================================

    private IEnumerator FadeInRoutine()
    {
        audioSource.Stop();


        audioSource.clip =
            bgm;


        audioSource.volume =
            0f;


        audioSource.loop =
            true;


        audioSource.Play();


        float elapsed =
            0f;


        while (
            elapsed <
            fadeInDuration
        )
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                fadeInDuration <= 0f
                ? 1f
                : elapsed /
                  fadeInDuration;


            audioSource.volume =
                Mathf.Lerp(
                    0f,
                    maxVolume,
                    t
                );


            yield return null;
        }


        audioSource.volume =
            maxVolume;


        fadeCoroutine =
            null;
    }


    // =====================================================
    // Fade Out Start
    // =====================================================

    private void StartFadeOutAndDestroy()
    {
        if (isEnding)
            return;


        isEnding =
            true;


        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }


        fadeCoroutine =
            StartCoroutine(
                FadeOutAndDestroyRoutine()
            );
    }


    // =====================================================
    // Fade Out
    // =====================================================

    private IEnumerator FadeOutAndDestroyRoutine()
    {
        if (
            audioSource != null &&
            audioSource.isPlaying
        )
        {
            float startVolume =
                audioSource.volume;


            float elapsed =
                0f;


            while (
                elapsed <
                fadeOutDuration
            )
            {
                elapsed +=
                    Time.unscaledDeltaTime;


                float t =
                    fadeOutDuration <= 0f
                    ? 1f
                    : elapsed /
                      fadeOutDuration;


                audioSource.volume =
                    Mathf.Lerp(
                        startVolume,
                        0f,
                        t
                    );


                yield return null;
            }


            audioSource.volume =
                0f;


            audioSource.Stop();
        }


        SceneManager.sceneLoaded -=
            OnSceneLoaded;


        if (Instance == this)
        {
            Instance =
                null;
        }


        Destroy(
            gameObject
        );
    }


    // =====================================================
    // Stop Immediately
    // =====================================================

    public void StopImmediately()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }


        if (audioSource != null)
        {
            audioSource.Stop();
        }


        SceneManager.sceneLoaded -=
            OnSceneLoaded;


        if (Instance == this)
        {
            Instance =
                null;
        }


        Destroy(
            gameObject
        );
    }


    // =====================================================
    // Destroy
    // =====================================================

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;


        if (Instance == this)
        {
            Instance =
                null;
        }
    }
}