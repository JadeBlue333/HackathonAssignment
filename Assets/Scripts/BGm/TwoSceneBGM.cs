using UnityEngine;
using UnityEngine.SceneManagement;

public class TwoSceneBGM : MonoBehaviour
{
    public static TwoSceneBGM Instance { get; private set; }


    // =====================================================
    // Audio
    // =====================================================

    [Header("Audio")]

    [SerializeField]
    private AudioSource audioSource;


    // =====================================================
    // Stop Scene
    // =====================================================

    [Header("Stop Scene")]

    [Tooltip("이 씬에 들어가면 BGM을 종료합니다.")]
    [SerializeField]
    private string stopSceneName = "Progress";


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
            return;


        audioSource.loop =
            true;


        if (!audioSource.isPlaying)
        {
            audioSource.Play();
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
        if (
            scene.name ==
            stopSceneName
        )
        {
            StopAndDestroy();
        }
    }


    // =====================================================
    // Stop
    // =====================================================

    public void StopAndDestroy()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }


        SceneManager.sceneLoaded -=
            OnSceneLoaded;


        Instance =
            null;


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