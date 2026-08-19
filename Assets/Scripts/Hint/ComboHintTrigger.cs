using System.Collections;
using UnityEngine;

public class ComboHintTrigger : MonoBehaviour
{
    // =========================================================
    // Hint
    // =========================================================

    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;


    // =========================================================
    // Message
    // =========================================================

    [Header("Message - Korean")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageKR =
        "연속 성공 보너스가 활성화되었습니다.\n연속 성공을 유지하면 신뢰도를 회복할 수 있습니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "The consecutive success bonus is now active.\nMaintain your streak to recover trust.";


    // =========================================================
    // Condition
    // =========================================================

    [Header("Condition")]

    [Tooltip("이 연속 성공 횟수에 도달하면 힌트를 표시합니다.")]
    [SerializeField]
    private int comboThreshold = 3;

    [Tooltip("힌트 표시 후 다시 표시할 수 있기까지의 최소 시간")]
    [SerializeField]
    private float cooldown = 5f;


    // =========================================================
    // Runtime
    // =========================================================

    private bool comboTriggered = false;

    private bool cooldownFinished = true;


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        int currentCombo =
            PlayerStatus.Instance.comboNumber;


        // -----------------------------------------------------
        // 콤보가 기준 아래로 내려가면 다시 표시 가능 상태 준비
        // -----------------------------------------------------

        if (currentCombo < comboThreshold)
        {
            comboTriggered = false;
        }


        // -----------------------------------------------------
        // 기준 달성 + 아직 이번 콤보에서 표시 안 함
        // + 쿨타임 종료 상태라면 표시
        // -----------------------------------------------------

        if (
            currentCombo >= comboThreshold &&
            !comboTriggered &&
            cooldownFinished
        )
        {
            ShowHint();
        }
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        comboTriggered = true;

        cooldownFinished = false;


        if (hintNotice != null)
        {
            hintNotice.Show(
                GetLocalizedMessage()
            );
        }


        StartCoroutine(
            CooldownRoutine()
        );
    }


    // =========================================================
    // Cooldown
    // =========================================================

    private IEnumerator CooldownRoutine()
    {
        if (cooldown > 0f)
        {
            yield return new WaitForSecondsRealtime(
                cooldown
            );
        }


        cooldownFinished = true;
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