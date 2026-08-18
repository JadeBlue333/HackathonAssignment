using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    [Header("Language")]
    public bool isEnglish = false;

    private CanvasGroup canvasKR;
    private CanvasGroup canvasEN;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SetupLanguageCanvases();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupLanguageCanvases();
    }

    private void SetupLanguageCanvases()
    {
        canvasKR = null;
        canvasEN = null;

        // 현재 씬의 모든 루트 오브젝트 가져오기
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            FindLanguageCanvas(root.transform);
        }

        UpdateCanvas();
    }

    private void FindLanguageCanvas(Transform parent)
    {
        // 현재 오브젝트 검사
        if (parent.name == "Canvas_KR")
        {
            canvasKR = parent.GetComponent<CanvasGroup>();

            if (canvasKR == null)
                canvasKR = parent.gameObject.AddComponent<CanvasGroup>();
        }
        else if (parent.name == "Canvas_EN")
        {
            canvasEN = parent.GetComponent<CanvasGroup>();

            if (canvasEN == null)
                canvasEN = parent.gameObject.AddComponent<CanvasGroup>();
        }

        // 자식들도 검사
        for (int i = 0; i < parent.childCount; i++)
        {
            FindLanguageCanvas(parent.GetChild(i));
        }
    }

    private void UpdateCanvas()
    {
        if (canvasKR != null)
        {
            canvasKR.alpha = isEnglish ? 0f : 1f;
            canvasKR.interactable = !isEnglish;
            canvasKR.blocksRaycasts = !isEnglish;
        }

        if (canvasEN != null)
        {
            canvasEN.alpha = isEnglish ? 1f : 0f;
            canvasEN.interactable = isEnglish;
            canvasEN.blocksRaycasts = isEnglish;
        }
    }

    public void SetKorean()
    {
        isEnglish = false;
        UpdateCanvas();
    }

    public void SetEnglish()
    {
        isEnglish = true;
        UpdateCanvas();
    }
}