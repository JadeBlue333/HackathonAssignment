using UnityEngine;

public class LowTrustHintTrigger : MonoBehaviour
{
    // =========================================================
    // Hint
    // =========================================================

    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [Tooltip("이 신뢰도 이하가 되면 힌트를 표시합니다.")]
    [SerializeField]
    private int trustThreshold = 20;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "신뢰도가 크게 낮아졌습니다.\n신뢰도가 0이 되면 폐기 처리됩니다.";


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 이미 한 번 표시했으면 종료
        if (PlayerStatus.Instance.lowTrustHintShown)
            return;


        // 신뢰도가 기준 이하이면 최초 1회 표시
        if (PlayerStatus.Instance.trust <= trustThreshold)
        {
            ShowHint();
        }
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        PlayerStatus.Instance.lowTrustHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                hintMessage
            );
        }
    }
}