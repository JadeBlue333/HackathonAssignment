using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToProgress : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField]
    private string progressSceneName = "Progress";


    // =========================================================
    // Progress로 복귀
    // 현재 작업은 완료하지 않은 것으로 처리
    // =========================================================

    public void ReturnProgress()
    {
        if (PlayerStatus.Instance != null)
        {
            // 이번 작업에서 발생한 임시 정산값 제거
            PlayerStatus.Instance.earnings = 0;
            PlayerStatus.Instance.trustChange = 0;

            // 이번 작업의 연속 성공 / 실수 기록 제거
            PlayerStatus.Instance.comboNumber = 0;
            PlayerStatus.Instance.mistakeNumber = 0;
        }


        if (string.IsNullOrEmpty(progressSceneName))
        {
            Debug.LogWarning(
                "Progress Scene Name이 비어있습니다."
            );

            return;
        }


        Debug.Log(
            $"현재 작업 취소 → {progressSceneName} 이동"
        );


        // 정산 / NextDay 없이 바로 이동
        SceneManager.LoadScene(
            progressSceneName
        );
    }
}