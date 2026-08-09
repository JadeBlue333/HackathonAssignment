using UnityEngine;

public class ActiveSyncUI : MonoBehaviour
{
    // =========================================================
    // Target
    // =========================================================

    [Header("Active Sync")]

    [Tooltip("이 오브젝트의 활성 상태를 기준으로 합니다.")]
    [SerializeField]
    private GameObject targetObject;

    [Tooltip("Target과 활성 상태를 같이 맞출 오브젝트")]
    [SerializeField]
    private GameObject syncObject;


    // =========================================================
    // Runtime
    // =========================================================

    private bool lastState;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (targetObject == null ||
            syncObject == null)
        {
            return;
        }


        bool currentState =
            targetObject.activeInHierarchy;


        // 상태가 바뀌었을 때만 갱신
        if (currentState != lastState)
        {
            Refresh();
        }
    }


    // =========================================================
    // Refresh
    // =========================================================

    private void Refresh()
    {
        if (targetObject == null ||
            syncObject == null)
        {
            return;
        }


        bool active =
            targetObject.activeInHierarchy;


        lastState =
            active;


        syncObject.SetActive(
            active
        );
    }
}