using UnityEngine;
using System.Collections;

public class HumanPartHintTrigger : MonoBehaviour
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
        "미등록 부품이 보관함에 추가되었습니다.\n[TAB]에서 확인할 수 있습니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "An unregistered part has been added to storage.\nYou can check it in [TAB].";


    // =========================================================
    // Delay
    // =========================================================

    [Header("Delay")]

    [Tooltip("Day 시작 후 힌트가 나타나기까지의 시간")]
    [SerializeField]
    private float showDelay = 1f;


    // =========================================================
    // Start
    // =========================================================

    private IEnumerator Start()
    {
        // =====================================================
        // PlayerStatus가 생성될 때까지 대기
        // =====================================================

        yield return new WaitUntil(
            () =>
                PlayerStatus.Instance != null
        );


        // =====================================================
        // 현재 인간 파츠 개수 확인
        // =====================================================

        int currentPartCount =
            PlayerStatus.Instance
                .GetHumanPartCount();


        int checkedPartCount =
            PlayerStatus.Instance
                .checkedHumanPartCount;


        // =====================================================
        // 이전에 확인한 개수보다 증가했는지 확인
        // =====================================================

        bool obtainedNewPart =
            currentPartCount >
            checkedPartCount;


        if (!obtainedNewPart)
            yield break;


        // =====================================================
        // Day 시작 후 일정 시간 대기
        // =====================================================

        yield return new WaitForSecondsRealtime(
            showDelay
        );


        // =====================================================
        // 힌트 표시
        // =====================================================

        ShowHint();


        // =====================================================
        // 현재 파츠 개수를 확인 완료 상태로 저장
        // =====================================================

        PlayerStatus.Instance
            .checkedHumanPartCount =
                currentPartCount;


        Debug.Log(
            $"인간 파츠 획득 힌트 표시 / " +
            $"이전: {checkedPartCount} / " +
            $"현재: {currentPartCount}"
        );
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
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