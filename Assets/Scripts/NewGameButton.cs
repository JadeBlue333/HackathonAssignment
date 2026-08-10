using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private GoToThisScene goToThisScene;
    [SerializeField] private GoToThisScene goToThisScene2;

    [Header("Reset Confirmation")]
    [SerializeField] private GameObject resetConfirmPanel;

    [Header("Continue Button")]
    [SerializeField] private Button continueButton;

    private bool canContinue = false;

    private void Start()
    {
        if (PlayerStatus.Instance != null && PlayerStatus.Instance.hasStarted)
        {
            canContinue = true;
        }
        else
        {
            canContinue = false;
        }
    }

    // =====================================================
    // New Game 버튼
    // =====================================================

    public void OnClickNewGame()
    {
        if (!canContinue)
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

    public void OnClickContinue()
    {
        if (canContinue)
        {
            goToThisScene2.nextSceneButton();
        }
        else
        {
            return;
        }
    }
}