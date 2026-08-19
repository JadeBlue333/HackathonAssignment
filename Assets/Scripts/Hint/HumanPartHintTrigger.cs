using UnityEngine;

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
    // Runtime
    // =========================================================

    private bool previousHead;
    private bool previousBody;
    private bool previousHeart;

    private bool initialized = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 현재 보유 상태를 기준값으로 저장
        // 이미 가지고 있던 파츠 때문에 힌트가 다시 뜨지 않게 함
        previousHead =
            PlayerStatus.Instance.humanHead;

        previousBody =
            PlayerStatus.Instance.humanBody;

        previousHeart =
            PlayerStatus.Instance.humanHeart;


        initialized = true;
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        if (!initialized)
        {
            previousHead =
                PlayerStatus.Instance.humanHead;

            previousBody =
                PlayerStatus.Instance.humanBody;

            previousHeart =
                PlayerStatus.Instance.humanHeart;

            initialized = true;

            return;
        }


        // -----------------------------------------------------
        // 새로운 파츠 획득 확인
        // -----------------------------------------------------

        bool obtainedNewPart =
            (!previousHead &&
             PlayerStatus.Instance.humanHead)
            ||
            (!previousBody &&
             PlayerStatus.Instance.humanBody)
            ||
            (!previousHeart &&
             PlayerStatus.Instance.humanHeart);


        // -----------------------------------------------------
        // 현재 상태 저장
        // -----------------------------------------------------

        previousHead =
            PlayerStatus.Instance.humanHead;

        previousBody =
            PlayerStatus.Instance.humanBody;

        previousHeart =
            PlayerStatus.Instance.humanHeart;


        // -----------------------------------------------------
        // 새로운 파츠를 얻었다면 힌트 표시
        // -----------------------------------------------------

        if (obtainedNewPart)
        {
            ShowHint();
        }
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