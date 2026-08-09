using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("씬 전환")]
    [Tooltip("Fuel이 0이 되었을 때 실행할 GoToThisScene")]
    [SerializeField] private GoToThisScene fuelZeroScene;

    [Tooltip("Trust가 0이 되었을 때 실행할 GoToThisScene")]
    [SerializeField] private GoToThisScene trustZeroScene;

    private bool sceneChanging = false;

    private void Update()
    {
        // PlayerStatus가 없으면 검사하지 않음
        if (PlayerStatus.Instance == null)
            return;

        // 이미 씬 전환을 실행했다면 다시 실행하지 않음
        if (sceneChanging)
            return;

        // ==========================================
        // 1. Fuel을 가장 먼저 검사
        // ==========================================
        if (PlayerStatus.Instance.fuel <= 0)
        {
            sceneChanging = true;

            if (fuelZeroScene != null)
            {
                fuelZeroScene.nextSceneButton();
            }
            else
            {
                Debug.LogWarning("Fuel Zero Scene에 연결된 GoToThisScene이 없습니다.");
            }
        }

        // ==========================================
        // 2. Fuel이 0이 아니면 Trust 검사
        // ==========================================
        if (PlayerStatus.Instance.trust + PlayerStatus.Instance.trustChange <= 0)
        {
            sceneChanging = true;

            if (trustZeroScene != null)
            {
                trustZeroScene.nextSceneButton();
            }
            else
            {
                Debug.LogWarning("Trust Zero Scene에 연결된 GoToThisScene이 없습니다.");
            }
        }
    }
}
