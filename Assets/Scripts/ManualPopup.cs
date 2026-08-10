using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ManualPopup : MonoBehaviour, IPointerClickHandler
{
    [Header("Manual Image")]
    [SerializeField] private Image manualImage;

    [Header("Manual Pages")]
    [SerializeField] private Sprite[] manualPages;

    private int currentPage = 0;


    private void Start()
    {
        ShowPage();
    }


    // =========================================================
    // UI Click
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭만 반응
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        NextPage();
    }


    // =========================================================
    // 다음 페이지
    // =========================================================

    private void NextPage()
    {
        if (manualPages == null ||
            manualPages.Length == 0)
            return;

        currentPage++;

        // 8 다음에는 다시 1
        if (currentPage >= manualPages.Length)
        {
            currentPage = 0;
        }

        ShowPage();
    }


    // =========================================================
    // 현재 페이지 표시
    // =========================================================

    private void ShowPage()
    {
        if (manualImage == null)
            return;

        if (manualPages == null ||
            manualPages.Length == 0)
            return;

        manualImage.sprite = manualPages[currentPage];
    }


    // =========================================================
    // 첫 페이지로 초기화
    // =========================================================

    public void ResetPage()
    {
        currentPage = 0;
        ShowPage();
    }
}