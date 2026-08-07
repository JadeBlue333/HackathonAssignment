using UnityEngine;

public class BlackMarketUIManager : MonoBehaviour
{
    [Header("Main Product Grid")]
    [SerializeField] private GameObject productGrid;

    [Header("Product Detail Panels")]
    [SerializeField] private GameObject[] productDetailPanels;

    private void Start()
    {
        ShowProductGrid();
    }

    // 상품 클릭
    public void ShowProductDetail(int index)
    {
        if (index < 0 || index >= productDetailPanels.Length)
        {
            Debug.LogWarning("잘못된 상품 인덱스: " + index);
            return;
        }

        // 상품 목록만 숨기기
        productGrid.SetActive(false);

        // 상세페이지 전부 끄기
        HideAllDetailPanels();

        // 선택한 상세페이지 켜기
        productDetailPanels[index].SetActive(true);
    }

    // 뒤로가기
    public void ShowProductGrid()
    {
        // 상세페이지 전부 끄기
        HideAllDetailPanels();

        // 상품 목록 다시 켜기
        productGrid.SetActive(true);
    }

    private void HideAllDetailPanels()
    {
        foreach (GameObject panel in productDetailPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}