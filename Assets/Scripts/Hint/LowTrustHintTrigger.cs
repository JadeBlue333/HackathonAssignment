using UnityEngine;

public class LowTrustHintTrigger : MonoBehaviour
{
    // =========================================================
    // Hint
    // =========================================================

    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [Tooltip("현재 신뢰도 + 누적 신뢰도 변화량이 이 값 이하가 되면 힌트를 표시합니다.")]
    [SerializeField]
    private int trustThreshold = 20;

    [Tooltip("신뢰도가 이 값 이상으로 회복되면 힌트를 다시 표시할 수 있습니다.")]
    [SerializeField]
    private int resetThreshold = 40;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message - Korean")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageKR =
        "신뢰도가 크게 낮아졌습니다.\n신뢰도가 0이 되면 폐기 처리됩니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "Trust has dropped significantly.\nIf trust reaches 0, you will be discarded.";


    // =========================================================
    // Runtime
    // =========================================================

    private bool canShowHint = true;


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 현재 신뢰도 + 오늘 누적 변화량
        int currentTrust =
            PlayerStatus.Instance.trust +
            PlayerStatus.Instance.trustChange;


        // -----------------------------------------------------
        // 40 이상으로 회복되면 다시 표시 가능
        // -----------------------------------------------------

        if (!canShowHint &&
            currentTrust >= resetThreshold)
        {
            canShowHint = true;
        }


        // -----------------------------------------------------
        // 20 이하가 되면 힌트 표시
        // -----------------------------------------------------

        if (canShowHint &&
            currentTrust <= trustThreshold)
        {
            ShowHint();
        }
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        canShowHint = false;


        if (hintNotice != null)
        {
            hintNotice.Show(
                GetLocalizedMessage()
            );
        }
    }


    // =========================================================
    // Localized Message
    // =========================================================

    private string GetLocalizedMessage()
    {
        if (LanguageManager.Instance == null)
        {
            return hintMessageKR;
        }


        return LanguageManager.Instance.isEnglish
            ? hintMessageEN
            : hintMessageKR;
    }
}