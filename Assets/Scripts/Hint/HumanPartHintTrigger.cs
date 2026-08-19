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
    // Save Key
    // =========================================================

    private const string HumanPartCountKey =
        "LastHumanPartCount";


    // =========================================================
    // Start
    // =========================================================

    private IEnumerator Start()
    {
        if (PlayerStatus.Instance == null)
            yield break;


        // =====================================================
        // 현재 보유 파츠 개수
        // =====================================================

        int currentPartCount =
            GetCurrentPartCount();


        // =====================================================
        // 이전 Day에서 확인했던 파츠 개수
        // =====================================================

        int previousPartCount =
            PlayerPrefs.GetInt(
                HumanPartCountKey,
                currentPartCount
            );


        // =====================================================
        // 파츠 개수가 증가했다면 새 파츠 획득
        // =====================================================

        bool obtainedNewPart =
            currentPartCount >
            previousPartCount;


        // =====================================================
        // 현재 개수를 다음 비교용으로 저장
        // =====================================================

        PlayerPrefs.SetInt(
            HumanPartCountKey,
            currentPartCount
        );

        PlayerPrefs.Save();


        // =====================================================
        // 새로운 파츠가 있다면 Day 시작 후 힌트 표시
        // =====================================================

        if (obtainedNewPart)
        {
            yield return new WaitForSecondsRealtime(
                showDelay
            );


            ShowHint();
        }
    }


    // =========================================================
    // Current Part Count
    // =========================================================

    private int GetCurrentPartCount()
    {
        if (PlayerStatus.Instance == null)
            return 0;


        int count =
            0;


        if (PlayerStatus.Instance.humanHead)
        {
            count++;
        }


        if (PlayerStatus.Instance.humanBody)
        {
            count++;
        }


        if (PlayerStatus.Instance.humanHeart)
        {
            count++;
        }


        return count;
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private void ShowHint()
    {
        if (hintNotice == null)
            return;


        hintNotice.Show(
            GetLocalizedMessage()
        );
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