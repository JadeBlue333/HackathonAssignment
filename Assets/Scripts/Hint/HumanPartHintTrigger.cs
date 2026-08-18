using UnityEngine;

public class HumanPartHintTrigger : MonoBehaviour
{
    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;


    [Header("Message")]

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "미등록 부품이 보관함에 추가되었습니다.\n[TAB]에서 확인할 수 있습니다.";


    private void Update()
    {
        if (PlayerStatus.Instance == null)
            return;


        // 이미 한 번 표시했으면 종료
        if (PlayerStatus.Instance.humanPartHintShown)
            return;


        // 인간 파츠를 하나라도 가지고 있다면 표시
        if (
            PlayerStatus.Instance.humanHead ||
            PlayerStatus.Instance.humanBody ||
            PlayerStatus.Instance.humanHeart
        )
        {
            ShowHint();
        }
    }


    private void ShowHint()
    {
        PlayerStatus.Instance.humanPartHintShown = true;


        if (hintNotice != null)
        {
            hintNotice.Show(
                hintMessage
            );
        }
    }
}