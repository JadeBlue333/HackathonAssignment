using UnityEngine;

public class FuelHintTrigger : MonoBehaviour
{
    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [SerializeField]
    private int fuelThreshold = 35;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message - Korean")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageKR =
        "임무는 [P] 업무 종료하기 (우측 상단)를 통해\n언제든 종료할 수 있습니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "You can end today's work at any time\nvia [P] End Work (top right).";


    // =========================================================
    // Update
    // =========================================================

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


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        PlayerStatus.Instance.fuelLowHintShown = true;


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