using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ContinueButtonGuard : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button button;


    // =====================================================
    // 데이터 상태에 따라 스크립트 ON / OFF
    // =====================================================

    [Header("게임 데이터 있음 → ON / 없음 → OFF")]
    [SerializeField] private MonoBehaviour[] enableWhenHasData;

    [Header("게임 데이터 있음 → OFF / 없음 → ON")]
    [SerializeField] private MonoBehaviour[] enableWhenNoData;


    // =====================================================
    // 클릭 이벤트
    // =====================================================

    [Header("게임 데이터 있을 때 클릭")]
    [SerializeField] private UnityEvent onHasDataClick;

    [Header("게임 데이터 없을 때 클릭")]
    [SerializeField] private UnityEvent onNoDataClick;


    private bool hasGameData;


    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    private void Start()
    {
        UpdateState();

        // 중요!
        // 클릭 자체는 항상 받을 수 있게 유지
        if (button != null)
            button.interactable = true;
    }


    // =====================================================
    // 상태 갱신
    // =====================================================

    public void UpdateState()
    {
        hasGameData = PlayerStatus.Instance != null;


        // 데이터 있음 → ON
        foreach (MonoBehaviour script in enableWhenHasData)
        {
            if (script != null)
                script.enabled = hasGameData;
        }


        // 데이터 없음 → ON
        foreach (MonoBehaviour script in enableWhenNoData)
        {
            if (script != null)
                script.enabled = !hasGameData;
        }
    }


    // =====================================================
    // 버튼 OnClick에 연결할 함수
    // =====================================================

    public void OnClickContinue()
    {
        if (hasGameData)
        {
            // 정상 이어하기
            onHasDataClick?.Invoke();
        }
        else
        {
            // 게임 데이터 없음
            onNoDataClick?.Invoke();
        }
    }
}