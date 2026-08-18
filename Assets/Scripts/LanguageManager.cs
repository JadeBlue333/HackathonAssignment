using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;


    // =========================================================
    // Language
    // =========================================================

    [Header("Language")]
    public bool isEnglish = false;


    // =========================================================
    // Language Canvas
    // =========================================================

    private CanvasGroup canvasKR;
    private CanvasGroup canvasEN;


    // =========================================================
    // Awake
    // =========================================================

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


        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );


        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        SetupLanguageCanvases();
    }


    // =========================================================
    // On Destroy
    // =========================================================

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    // =========================================================
    // Scene Loaded
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SetupLanguageCanvases();
    }


    // =========================================================
    // Setup Language Canvas
    // =========================================================

    private void SetupLanguageCanvases()
    {
        canvasKR = null;
        canvasEN = null;


        GameObject[] roots =
            SceneManager
                .GetActiveScene()
                .GetRootGameObjects();


        foreach (GameObject root in roots)
        {
            FindLanguageCanvas(
                root.transform
            );
        }


        UpdateCanvas();
    }


    // =========================================================
    // Find Language Canvas
    // =========================================================

    private void FindLanguageCanvas(
        Transform parent
    )
    {
        if (parent.name == "Canvas_KR")
        {
            canvasKR =
                parent.GetComponent<CanvasGroup>();


            if (canvasKR == null)
            {
                canvasKR =
                    parent.gameObject
                        .AddComponent<CanvasGroup>();
            }
        }

        else if (parent.name == "Canvas_EN")
        {
            canvasEN =
                parent.GetComponent<CanvasGroup>();


            if (canvasEN == null)
            {
                canvasEN =
                    parent.gameObject
                        .AddComponent<CanvasGroup>();
            }
        }


        for (
            int i = 0;
            i < parent.childCount;
            i++
        )
        {
            FindLanguageCanvas(
                parent.GetChild(i)
            );
        }
    }


    // =========================================================
    // Update Canvas
    // =========================================================

    private void UpdateCanvas()
    {
        // -----------------------------------------------------
        // Korean Canvas
        // -----------------------------------------------------

        if (canvasKR != null)
        {
            canvasKR.alpha =
                isEnglish ? 0f : 1f;

            canvasKR.interactable =
                !isEnglish;

            canvasKR.blocksRaycasts =
                !isEnglish;
        }


        // -----------------------------------------------------
        // English Canvas
        // -----------------------------------------------------

        if (canvasEN != null)
        {
            canvasEN.alpha =
                isEnglish ? 1f : 0f;

            canvasEN.interactable =
                isEnglish;

            canvasEN.blocksRaycasts =
                isEnglish;
        }
    }


    // =========================================================
    // Set Korean
    // =========================================================

    public void SetKorean()
    {
        isEnglish = false;

        UpdateCanvas();
    }


    // =========================================================
    // Set English
    // =========================================================

    public void SetEnglish()
    {
        isEnglish = true;

        UpdateCanvas();
    }
}