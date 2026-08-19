using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuProgressBGM : MonoBehaviour
{
    public static MenuProgressBGM Instance { get; private set; }


    // =====================================================
    // Scene
    // =====================================================

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string progressSceneName = "Progress";
    [SerializeField] private string introSceneName = "Intro";
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private string tutorialENSceneName = "Tutorial_EN";


    // =====================================================
    // Audio
    // =====================================================

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bgmClip;

    [Range(0f, 1f)]
    [SerializeField] private float normalVolume = 0.5f;


    // =====================================================
    // Fade
    // =====================================================

    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 1.5f;

    private Coroutine fadeCoroutine;


    // =====================================================
    // Awake
    // =====================================================

    private void Awake()
    {
        // 이미 기존 BGM이 살아있다면
        // 새로 생성된 BGM 오브젝트는 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }


        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }
    }


    // =====================================================
    // Start
    // =====================================================

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬 상태도 한 번 바로 확인
        HandleScene(SceneManager.GetActiveScene().name);
    }


    // =====================================================
    // Scene Loaded
    // =====================================================

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleScene(scene.name);
    }


    // =====================================================
    // Scene 처리
    // =====================================================

    private void HandleScene(string sceneName)
    {
        // =================================================
        // Intro
        // =================================================

        if (sceneName == introSceneName)
        {
            // 음악은 계속 재생하지만 소리만 안 들리게
            if (audioSource != null)
            {
                audioSource.volume = 0f;
            }

            return;
        }


        // =================================================
        // MainMenu / Progress / Tutorial / Tutorial_EN
        // =================================================

        if (
            sceneName == mainMenuSceneName ||
            sceneName == progressSceneName ||
            sceneName == tutorialSceneName ||
            sceneName == tutorialENSceneName
        )
        {
            // Fade Out 중이었다면 취소
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }


            if (audioSource == null ||
                bgmClip == null)
            {
                return;
            }


            // Intro에서 0으로 만들어뒀던 볼륨 복구
            audioSource.volume = normalVolume;


            // 이미 재생 중이면 그대로 이어서 재생
            if (audioSource.isPlaying)
            {
                return;
            }


            // 재생 중이 아니면 처음부터 시작
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.Play();

            return;
        }


        // =================================================
        // 그 외 Scene
        // =================================================

        if (audioSource != null &&
            audioSource.isPlaying)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine =
                StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            DestroyBGM();
        }
    }


    // =====================================================
    // Fade Out
    // =====================================================

    private IEnumerator FadeOutAndDestroy()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;


        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / fadeOutDuration
                );

            audioSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t
                );

            yield return null;
        }


        audioSource.volume = 0f;
        audioSource.Stop();

        fadeCoroutine = null;

        DestroyBGM();
    }


    // =====================================================
    // Destroy
    // =====================================================

    private void DestroyBGM()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }
}