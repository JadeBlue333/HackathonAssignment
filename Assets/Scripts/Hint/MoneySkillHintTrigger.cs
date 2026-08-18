using UnityEngine;

public class MoneySkillHintTrigger : MonoBehaviour
{
    // =========================================================
    // Hint
    // =========================================================

    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [Tooltip("이 금액 이상을 처음 보유하면 힌트 표시")]
    [SerializeField]
    private int moneyThreshold = 20;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "[TAB] 기술 강화 / 도구 보기에서 능력을 강화할 수 있습니다.";


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 이미 한 번 보여줬으면 종료
        if (PlayerStatus.Instance.moneySkillHintShown)
            return;


        // 보유 금액이 기준 이상이면 최초 1회 표시
        if (PlayerStatus.Instance.money >= moneyThreshold)
        {
            ShowHint();
        }
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        PlayerStatus.Instance.moneySkillHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                hintMessage
            );
        }
    }
}