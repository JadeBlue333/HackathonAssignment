using System.Collections;
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

    [Tooltip("조건 성립 후 힌트가 나타나기까지의 시간")]
    [SerializeField]
    private float showDelay = 1f;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message - Korean")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageKR =
        "[TAB] 기술 강화 / 도구 보기에서\n능력을 강화할 수 있습니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "You can upgrade your abilities in\n[TAB] Upgrade Skills / Tools.";


    // =========================================================
    // Runtime
    // =========================================================

    private bool isWaiting = false;


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


        // 이미 표시 대기 중이면 종료
        if (isWaiting)
            return;


        // 보유 금액이 기준 이상이면 최초 1회 대기 시작
        if (PlayerStatus.Instance.money >= moneyThreshold)
        {
            StartCoroutine(
                ShowHintAfterDelay()
            );
        }
    }


    // =========================================================
    // Show Hint After Delay
    // =========================================================

    private IEnumerator ShowHintAfterDelay()
    {
        isWaiting = true;


        if (showDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                showDelay
            );
        }


        ShowHint();
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        if (PlayerStatus.Instance == null)
        {
            isWaiting = false;
            return;
        }


        // 다른 곳에서 이미 표시 처리된 경우
        if (PlayerStatus.Instance.moneySkillHintShown)
        {
            isWaiting = false;
            return;
        }


        PlayerStatus.Instance.moneySkillHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                GetLocalizedMessage()
            );
        }


        isWaiting = false;
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