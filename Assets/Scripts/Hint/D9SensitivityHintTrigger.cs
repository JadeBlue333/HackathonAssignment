using System.Collections;
using UnityEngine;

public class D9SensitivityHintTrigger : MonoBehaviour
{
    [Header("Hint")]

    [SerializeField]
    private HintNotice hintNotice;

    [TextArea(2, 5)]
    [SerializeField]
    private string hintMessage =
        "[ESC] 설정에서 물체 회전 감도를 조절할 수 있습니다.";

    [Tooltip("D-9 시작 후 힌트가 나타나기까지의 시간")]
    [SerializeField]
    private float showDelay = 2f;


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
                hintMessage
            );
        }
    }
}