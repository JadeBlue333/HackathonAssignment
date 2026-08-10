using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuProgressBGM : MonoBehaviour
{
    public static MenuProgressBGM Instance { get; private set; }


    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string progressSceneName = "Progress";


    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bgmClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;


    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 1.5f;


    private Coroutine fadeCoroutine;


    private void Awake()
    {
        // MainMenu ↔ Progress 이동 시
        // 기존 BGM이 살아있으면 새로 생성된 BGM 제거
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
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }


    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;

        PlayBGM();
    }


    // =========================================================
    // Scene Change
    // =========================================================

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        string oldSceneName = oldScene.name;
        string newSceneName = newScene.name;


        bool oldIsMenuScene = IsMenuScene(oldSceneName);
        bool newIsMenuScene = IsMenuScene(newSceneName);


        // =====================================================
        // MainMenu ↔ Progress
        // =====================================================

        if (oldIsMenuScene && newIsMenuScene)
        {
            // 아무것도 하지 않음.
            // 현재 재생 위치 그대로 계속 재생.
            return;
        }


        // =====================================================
        // MainMenu / Progress → 다른 Scene
        // =====================================================

        if (oldIsMenuScene && !newIsMenuScene)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine =
                StartCoroutine(FadeOutAndDestroy());

            return;
        }
    }


    // =========================================================
    // Menu Scene Check
    // =========================================================

    private bool IsMenuScene(string sceneName)
    {
        return
            sceneName == mainMenuSceneName ||
            sceneName == progressSceneName;
    }


    // =========================================================
    // Play
    // =========================================================

    private void PlayBGM()
    {
        if (audioSource == null ||
            bgmClip == null)
        {
            return;
        }


        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = true;


        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }


    // =========================================================
    // Fade Out
    // =========================================================

    private IEnumerator FadeOutAndDestroy()
    {
        if (audioSource == null)
        {
            Destroy(gameObject);
            yield break;
        }


        float startVolume = audioSource.volume;
        float elapsed = 0f;


        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                elapsed / fadeOutDuration;

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


        if (Instance == this)
        {
            Instance = null;
        }

        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;

        if (Instance == this)
        {
            Instance = null;
        }
    }
}