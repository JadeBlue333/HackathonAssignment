using UnityEngine;

public class BlackMarketUIManager : MonoBehaviour
{
    [Header("Main page")]
    [SerializeField] private GameObject shopMainPanel;

    [Header("Product detail pages")]
    [SerializeField] private GameObject[] productDetailPanels;

    private void Start()
    {
        ShowMainPage();
    }

    public void ShowProductDetail(int productIndex)
    {
        if (productIndex < 0 || productIndex >= productDetailPanels.Length)
        {
            Debug.LogWarning($"잘못된 상품 번호입니다: {productIndex}");
            return;
        }

        shopMainPanel.SetActive(false);
        HideAllDetailPanels();

        productDetailPanels[productIndex].SetActive(true);
    }

    public void ShowMainPage()
    {
        shopMainPanel.SetActive(true);
        HideAllDetailPanels();
    }

    private void HideAllDetailPanels()
    {
        foreach (GameObject detailPanel in productDetailPanels)
        {
            if (detailPanel != null)
            {
                detailPanel.SetActive(false);
            }
        }
    }
}