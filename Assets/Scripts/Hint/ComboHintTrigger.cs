using UnityEngine;

public class ComboHintTrigger : MonoBehaviour
{
    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;


    [Header("Message")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "연속 성공 보너스가 활성화되었습니다.\n연속 성공을 유지하면 신뢰도를 회복할 수 있습니다.";


    [Header("Condition")]

    [Tooltip("이 연속 성공 횟수에 도달하면 힌트를 표시합니다.")]
    [SerializeField]
    private int comboThreshold = 3;


    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 이미 한 번 표시했으면 종료
        if (PlayerStatus.Instance.comboHintShown)
            return;


        if (PlayerStatus.Instance.comboNumber >= comboThreshold)
        {
            ShowHint();
        }
    }


    private void ShowHint()
    {
        PlayerStatus.Instance.comboHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                hintMessage
            );
        }
    }
}