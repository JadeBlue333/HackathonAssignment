using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private GoToThisScene goToThisScene;

    [Header("Reset Confirmation")]
    [SerializeField] private GameObject resetConfirmPanel;

    [Header("Continue Button")]
    [SerializeField] private Button continueButton;

    private void Start()
    {
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        // PlayerStatus가 존재하면 Continue 활성화
        // 없으면 비활성화
        continueButton.interactable = PlayerStatus.Instance != null;
    }

    // =====================================================
    // New Game 버튼
    // =====================================================

    public void OnClickNewGame()
    {
        // PlayerStatus가 없음 = 처음 게임 실행
        if (PlayerStatus.Instance == null)
        {
            goToThisScene.nextSceneButton();
            return;
        }

        // 기존 게임 데이터가 있음
        resetConfirmPanel.SetActive(true);
    }

    // =====================================================
    // 초기화 확인 - 예
    // =====================================================

    public void ConfirmNewGame()
    {
        if (PlayerStatus.Instance != null)
        {
            Destroy(PlayerStatus.Instance.gameObject);
        }

        resetConfirmPanel.SetActive(false);

        goToThisScene.nextSceneButton();
    }

    // =====================================================
    // 초기화 확인 - 아니오
    // =====================================================

    public void CancelNewGame()
    {
        resetConfirmPanel.SetActive(false);
    }
}