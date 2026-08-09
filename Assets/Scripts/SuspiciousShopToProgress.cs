using UnityEngine;
using UnityEngine.SceneManagement;

public class SuspiciousShopToProgress : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField]
    private string progressSceneName = "Progress";


    public void GoToProgress()
    {
        if (string.IsNullOrEmpty(progressSceneName))
        {
            Debug.LogWarning(
                "Progress Scene Name이 비어있습니다."
            );

            return;
        }


        if (PlayerStatus.Instance == null)
        {
            Debug.LogError(
                "PlayerStatus.Instance가 없습니다."
            );

            return;
        }


        Debug.Log(
            $"수상한 상점 종료 → {progressSceneName} 이동"
        );


        // PlayerStatus 데이터는 건드리지 않음.
        // DontDestroyOnLoad 상태 그대로 Progress로 이동.
        SceneManager.LoadScene(
            progressSceneName
        );
    }
}