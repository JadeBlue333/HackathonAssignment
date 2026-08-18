using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    // Tagged Objects
    // =========================================================

    private readonly List<GameObject> koreanObjects =
        new List<GameObject>();

    private readonly List<GameObject> englishObjects =
        new List<GameObject>();


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
        SetupLanguageObjects();
    }


    // =========================================================
    // On Destroy
    // =========================================================

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;
        }
    }


    // =========================================================
    // Scene Loaded
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        SetupLanguageObjects();
    }


    // =========================================================
    // Setup Language Objects
    // =========================================================

    private void SetupLanguageObjects()
    {
        canvasKR = null;
        canvasEN = null;


        koreanObjects.Clear();
        englishObjects.Clear();


        GameObject[] roots =
            SceneManager
                .GetActiveScene()
                .GetRootGameObjects();


        foreach (GameObject root in roots)
        {
            FindLanguageObjects(
                root.transform
            );
        }


        UpdateLanguage();
    }


    // =========================================================
    // Find Language Objects
    // =========================================================

    private void FindLanguageObjects(
        Transform parent
    )
    {
        GameObject obj =
            parent.gameObject;


        // -----------------------------------------------------
        // Korean Canvas
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // English Canvas
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // Korean Tag
        // -----------------------------------------------------

        if (obj.CompareTag("kor"))
        {
            koreanObjects.Add(
                obj
            );
        }


        // -----------------------------------------------------
        // English Tag
        // -----------------------------------------------------

        else if (obj.CompareTag("eng"))
        {
            englishObjects.Add(
                obj
            );
        }


        // -----------------------------------------------------
        // Children
        // -----------------------------------------------------

        for (
            int i = 0;
            i < parent.childCount;
            i++
        )
        {
            FindLanguageObjects(
                parent.GetChild(i)
            );
        }
    }


    // =========================================================
    // Update Language
    // =========================================================

    private void UpdateLanguage()
    {
        UpdateCanvas();
        UpdateTaggedObjects();
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
    // Update Tagged Objects
    // =========================================================

    private void UpdateTaggedObjects()
    {
        // -----------------------------------------------------
        // Korean Objects
        // -----------------------------------------------------

        foreach (GameObject obj in koreanObjects)
        {
            if (obj == null)
            {
                continue;
            }


            obj.SetActive(
                !isEnglish
            );
        }


        // -----------------------------------------------------
        // English Objects
        // -----------------------------------------------------

        foreach (GameObject obj in englishObjects)
        {
            if (obj == null)
            {
                continue;
            }


            obj.SetActive(
                isEnglish
            );
        }
    }


    // =========================================================
    // Set Korean
    // =========================================================

    public void SetKorean()
    {
        isEnglish = false;

        UpdateLanguage();
    }


    // =========================================================
    // Set English
    // =========================================================

    public void SetEnglish()
    {
        isEnglish = true;

        UpdateLanguage();
    }
}