using System.Collections;
using UnityEngine;

public class D9SensitivityHintTrigger : MonoBehaviour
{
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
        "[ESC] 설정 (좌측 상단 톱니바퀴)에서\n물체 회전 감도를 조절할 수 있습니다.";


    [Header("Message - English")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessageEN =
        "You can adjust object rotation sensitivity in\n[ESC] Settings (gear icon, top left).";


    [Tooltip("D-9 시작 후 힌트가 나타나기까지의 시간")]
    [SerializeField]
    private float showDelay = 2f;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (PlayerStatus.Instance == null)
        {
            return;
        }

        // D-9에서만 표시
        if (PlayerStatus.Instance.currentDay != 9)
        {
            return;
        }

        StartCoroutine(
            ShowHintAfterDelay()
        );
    }


    // =========================================================
    // Show Hint
    // =========================================================

    private IEnumerator ShowHintAfterDelay()
    {
        if (showDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                showDelay
            );
        }


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