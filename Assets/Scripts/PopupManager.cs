using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }


    // =========================================================
    // Runtime
    // =========================================================

    private readonly List<GameObject> popups =
        new List<GameObject>();

    private GameObject lastOpenedPopup;


    // =========================================================
    // 자동 생성
    // =========================================================

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.AfterSceneLoad
    )]
    private static void CreateAutomatically()
    {
        if (Instance != null)
            return;


        GameObject managerObject =
            new GameObject(
                "PopupManager"
            );


        managerObject.AddComponent<
            PopupManager
        >();
    }


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
            Destroy(
                gameObject
            );

            return;
        }


        Instance =
            this;


        DontDestroyOnLoad(
            gameObject
        );


        SceneManager.sceneLoaded +=
            OnSceneLoaded;


        RefreshPopups();
    }


    // =========================================================
    // Destroy
    // =========================================================

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


    // =========================================================
    // Scene Loaded
    // =========================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        lastOpenedPopup =
            null;


        RefreshPopups();
    }


    // =========================================================
    // 현재 씬 Popup 자동 검색
    // =========================================================

    private void RefreshPopups()
    {
        popups.Clear();


        PopupMarker[] markers =
            FindObjectsByType<PopupMarker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (
            PopupMarker marker in markers
        )
        {
            if (marker == null)
                continue;


            // DontDestroyOnLoad에 남아있는
            // 다른 씬 오브젝트 방지
            if (
                marker.gameObject.scene !=
                SceneManager.GetActiveScene()
            )
            {
                continue;
            }


            popups.Add(
                marker.gameObject
            );
        }


        Debug.Log(
            $"PopupManager : " +
            $"{popups.Count}개의 팝업을 찾았습니다."
        );
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        CheckPopups();
    }


    // =========================================================
    // 팝업 상태 검사
    // =========================================================

    private void CheckPopups()
    {
        GameObject newlyOpenedPopup =
            null;


        // =====================================================
        // 새로 열린 팝업 찾기
        // =====================================================

        foreach (
            GameObject popup in popups
        )
        {
            if (popup == null)
                continue;


            if (!popup.activeSelf)
                continue;


            if (
                popup !=
                lastOpenedPopup
            )
            {
                newlyOpenedPopup =
                    popup;
            }
        }


        // =====================================================
        // 새 팝업 발견
        // =====================================================

        if (
            newlyOpenedPopup != null
        )
        {
            foreach (
                GameObject popup in popups
            )
            {
                if (popup == null)
                    continue;


                if (
                    popup ==
                    newlyOpenedPopup
                )
                {
                    continue;
                }


                popup.SetActive(
                    false
                );
            }


            lastOpenedPopup =
                newlyOpenedPopup;


            return;
        }


        // =====================================================
        // 현재 팝업이 닫힌 경우
        // =====================================================

        if (
            lastOpenedPopup != null &&
            !lastOpenedPopup.activeSelf
        )
        {
            lastOpenedPopup =
                null;
        }
    }


    // =========================================================
    // 직접 Popup 열기
    // =========================================================

    public void OpenPopup(
        GameObject targetPopup
    )
    {
        if (targetPopup == null)
            return;


        foreach (
            GameObject popup in popups
        )
        {
            if (popup == null)
                continue;


            popup.SetActive(
                popup ==
                targetPopup
            );
        }


        lastOpenedPopup =
            targetPopup;
    }


    // =========================================================
    // 특정 Popup 닫기
    // =========================================================

    public void ClosePopup(
        GameObject targetPopup
    )
    {
        if (targetPopup == null)
            return;


        targetPopup.SetActive(
            false
        );


        if (
            lastOpenedPopup ==
            targetPopup
        )
        {
            lastOpenedPopup =
                null;
        }
    }


    // =========================================================
    // 전체 Popup 닫기
    // =========================================================

    public void CloseAllPopups()
    {
        foreach (
            GameObject popup in popups
        )
        {
            if (popup == null)
                continue;


            popup.SetActive(
                false
            );
        }


        lastOpenedPopup =
            null;
    }


    // =========================================================
    // Popup이 하나라도 열려있는지
    // =========================================================

    public bool HasOpenPopup()
    {
        foreach (
            GameObject popup in popups
        )
        {
            if (
                popup != null &&
                popup.activeSelf
            )
            {
                return true;
            }
        }


        return false;
    }
}