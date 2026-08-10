using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThreeSceneBGM : MonoBehaviour
{
    public static ThreeSceneBGM Instance { get; private set; }


    // =====================================================
    // Audio
    // =====================================================

    [Header("Audio")]

    [SerializeField]
    private AudioSource audioSource;


    // =====================================================
    // Scene 1
    // =====================================================

    [Header("Scene 1")]

    [SerializeField]
    private string scene1Name = "D-3";

    [SerializeField]
    private AudioClip scene1BGM;


    // =====================================================
    // Scene 2
    // =====================================================

    [Header("Scene 2")]

    [SerializeField]
    private string scene2Name = "SuspiciousShop";

    [SerializeField]
    private AudioClip scene2BGM;


    // =====================================================
    // Scene 3
    // =====================================================

    [Header("Scene 3")]

    [Tooltip("이 씬으로 갈 때 BGM을 페이드아웃하고 종료합니다.")]
    [SerializeField]
    private string scene3Name = "Progress";


    // =====================================================
    // Fade
    // =====================================================

    [Header("Fade")]

    [Tooltip("BGM이 켜질 때 걸리는 시간")]
    [SerializeField]
    private float fadeInDuration = 1.5f;

    [Tooltip("BGM이 꺼질 때 걸리는 시간")]
    [SerializeField]
    private float fadeOutDuration = 1.5f;

    [Range(0f, 1f)]
    [SerializeField]
    private float maxVolume = 1f;


    // =====================================================
    // Runtime
    // =====================================================

    private Coroutine fadeCoroutine;

    private bool isChangingScene = false;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );


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


        audioSource.loop =
            true;


        string currentScene =
            SceneManager
                .GetActiveScene()
                .name;


        // 씬1에서 시작
        if (currentScene == scene1Name)
        {
            PlayWithFade(
                scene1BGM
            );
        }

        // 테스트용으로 씬2에서 바로 실행했을 경우
        else if (currentScene == scene2Name)
        {
            PlayWithFade(
                scene2BGM
            );
        }
    }


    // =====================================================
    // Scene Loaded
    // =====================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (isChangingScene)
            return;


        // 씬2에 들어오면 음악 교체
        if (scene.name == scene2Name)
        {
            ChangeBGM(
                scene2BGM
            );
        }


        // 혹시 다른 방식으로 씬3에 들어갔을 경우
        // BGM 즉시 종료
        else if (scene.name == scene3Name)
        {
            StopAndDestroy();
        }
    }


    // =====================================================
    // Play
    // =====================================================

    private void PlayWithFade(
        AudioClip clip
    )
    {
        if (clip == null ||
            audioSource == null)
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
                FadeInRoutine(
                    clip
                )
            );
    }


    // =====================================================
    // Change BGM
    // =====================================================

    private void ChangeBGM(
        AudioClip newClip
    )
    {
        if (newClip == null ||
            audioSource == null)
        {
            return;
        }


        // 이미 같은 음악이면 아무것도 안 함
        if (audioSource.clip == newClip &&
            audioSource.isPlaying)
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
                ChangeBGMRoutine(
                    newClip
                )
            );
    }


    // =====================================================
    // Scene 2 → Scene 3
    // =====================================================

    public void FadeOutAndLoadScene3()
    {
        if (isChangingScene)
            return;


        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }


        fadeCoroutine =
            StartCoroutine(
                FadeOutAndLoadRoutine(
                    scene3Name
                )
            );
    }


    // =====================================================
    // 원하는 씬으로 Fade Out 후 이동
    // =====================================================

    public void FadeOutAndLoadScene(
        string sceneName
    )
    {
        if (isChangingScene)
            return;


        if (fadeCoroutine != null)
        {
            StopCoroutine(
                fadeCoroutine
            );
        }


        fadeCoroutine =
            StartCoroutine(
                FadeOutAndLoadRoutine(
                    sceneName
                )
            );
    }


    // =====================================================
    // Fade In
    // =====================================================

    private IEnumerator FadeInRoutine(
        AudioClip clip
    )
    {
        audioSource.Stop();

        audioSource.clip =
            clip;

        audioSource.volume =
            0f;

        audioSource.loop =
            true;

        audioSource.Play();


        float elapsed =
            0f;


        while (elapsed < fadeInDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                fadeInDuration <= 0f
                ? 1f
                : elapsed / fadeInDuration;


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
    // BGM 교체
    // =====================================================

    private IEnumerator ChangeBGMRoutine(
        AudioClip newClip
    )
    {
        // 기존 음악 Fade Out
        float startVolume =
            audioSource.volume;

        float elapsed =
            0f;


        while (elapsed < fadeOutDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                fadeOutDuration <= 0f
                ? 1f
                : elapsed / fadeOutDuration;


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


        // 새로운 음악으로 교체
        audioSource.clip =
            newClip;

        audioSource.loop =
            true;

        audioSource.Play();


        // 새로운 음악 Fade In
        elapsed =
            0f;


        while (elapsed < fadeInDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                fadeInDuration <= 0f
                ? 1f
                : elapsed / fadeInDuration;


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
    // Fade Out → Scene 이동
    // =====================================================

    private IEnumerator FadeOutAndLoadRoutine(
        string sceneName
    )
    {
        isChangingScene =
            true;


        if (audioSource != null &&
            audioSource.isPlaying)
        {
            float startVolume =
                audioSource.volume;

            float elapsed =
                0f;


            while (elapsed < fadeOutDuration)
            {
                elapsed +=
                    Time.unscaledDeltaTime;


                float t =
                    fadeOutDuration <= 0f
                    ? 1f
                    : elapsed / fadeOutDuration;


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


        Instance =
            null;


        // 자신을 없애기 전에 씬 이름 저장
        string targetScene =
            sceneName;


        Destroy(
            gameObject
        );


        SceneManager.LoadScene(
            targetScene
        );
    }


    // =====================================================
    // Stop
    // =====================================================

    public void StopAndDestroy()
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