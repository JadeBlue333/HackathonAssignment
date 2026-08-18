using UnityEngine;

public class FuelHintTrigger : MonoBehaviour
{
    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [SerializeField]
    private int fuelThreshold = 35;


    [Header("Message")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "연료가 부족합니다.\n[P]로 오늘의 업무를 종료할 수 있습니다.";


    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        if (PlayerStatus.Instance.fuelLowHintShown)
            return;


        if (PlayerStatus.Instance.fuel <= fuelThreshold)
        {
            ShowHint();
        }
    }


    private void ShowHint()
    {
        PlayerStatus.Instance.fuelLowHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                hintMessage
            );
        }
    }
}