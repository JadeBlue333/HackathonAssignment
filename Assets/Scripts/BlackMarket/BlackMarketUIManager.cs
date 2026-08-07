using UnityEngine;

public class BlackMarketUIManager : MonoBehaviour
{
    [Header("Main Product Grid")]
    [SerializeField] private GameObject productGrid;

    [Header("Product Detail Panels")]
    [SerializeField] private GameObject[] productDetailPanels;

    [Header("Additional UI Panels")]
    [SerializeField] private GameObject[] additionalUIPanels;

    [Header("Panels To Hide When Opening Additional UI")]
    [SerializeField] private GameObject[] panelsToHideWhenOpeningUI;


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


    // =========================
    // 추가 UI 패널 열기
    // =========================
    public void OpenUIPanel(int index)
    {
        if (index < 0 || index >= additionalUIPanels.Length)
        {
            Debug.LogWarning("잘못된 UI 패널 인덱스: " + index);
            return;
        }

        // Inspector에서 지정한 패널들 끄기
        foreach (GameObject panel in panelsToHideWhenOpeningUI)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        // 선택한 추가 UI 열기
        if (additionalUIPanels[index] != null)
        {
            additionalUIPanels[index].SetActive(true);
        }
    }

    // =========================
    // 추가 UI 패널 닫기
    // =========================
    public void CloseUIPanel(int index)
    {
        if (index < 0 || index >= additionalUIPanels.Length)
        {
            Debug.LogWarning("잘못된 UI 패널 인덱스: " + index);
            return;
        }

        if (additionalUIPanels[index] != null)
        {
            additionalUIPanels[index].SetActive(false);
        }
    }
}