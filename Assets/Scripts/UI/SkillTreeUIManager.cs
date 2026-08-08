using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeUIManager : MonoBehaviour
{
    public static SkillTreeUIManager Instance
    {
        get;
        private set;
    }


    // =========================================================
    // Detail Panel
    // =========================================================

    [Header("Detail Panel")]

    [Tooltip("아래쪽 상품 설명 전체 Panel")]
    [SerializeField]
    private GameObject detailPanel;


    [Header("Texts")]

    [SerializeField]
    private TMP_Text skillNameText;

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private TMP_Text priceText;


    // =========================================================
    // Purchase Button
    // =========================================================

    [Header("Purchase Button")]

    [SerializeField]
    private Button purchaseButton;

    [SerializeField]
    private TMP_Text purchaseButtonText;


    // =========================================================
    // Runtime
    // =========================================================

    private SkillNodeUI selectedNode;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        Instance = this;


        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(
                PurchaseSelectedSkill
            );
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        // 처음에는 설명창 숨김
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }


        RefreshAllNodes();
    }


    // =========================================================
    // Select
    // =========================================================

    public void SelectNode(
        SkillNodeUI node
    )
    {
        if (node == null)
            return;


        selectedNode =
            node;


        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
        }


        RefreshDetailPanel();
    }


    // =========================================================
    // Detail
    // =========================================================

    private void RefreshDetailPanel()
    {
        if (selectedNode == null)
            return;


        // ---------------------------------------------
        // 이름
        // ---------------------------------------------

        if (skillNameText != null)
        {
            skillNameText.text =
                selectedNode.skillName;
        }


        // ---------------------------------------------
        // 설명
        // ---------------------------------------------

        if (descriptionText != null)
        {
            descriptionText.text =
                selectedNode.description;
        }


        // ---------------------------------------------
        // 가격
        // ---------------------------------------------

        if (priceText != null)
        {
            priceText.text =
                $"{selectedNode.price} C";
        }


        // ---------------------------------------------
        // 구매 상태
        // ---------------------------------------------

        RefreshPurchaseButton();
    }


    // =========================================================
    // Purchase Button
    // =========================================================

    private void RefreshPurchaseButton()
    {
        if (selectedNode == null)
            return;

        if (purchaseButton == null)
            return;


        // =============================================
        // 이미 구매됨
        // =============================================

        if (selectedNode.IsPurchased())
        {
            purchaseButton.interactable =
                false;

            SetPurchaseButtonText(
                "구매완료"
            );

            return;
        }


        // =============================================
        // 이전 단계 미구매
        // =============================================

        if (!selectedNode.IsPreviousLevelPurchased())
        {
            purchaseButton.interactable =
                false;

            SetPurchaseButtonText(
                "구매불가"
            );

            return;
        }


        // =============================================
        // 돈 부족
        // =============================================

        if (PlayerStatus.Instance == null ||
            PlayerStatus.Instance.money <
            selectedNode.price)
        {
            purchaseButton.interactable =
                false;

            SetPurchaseButtonText(
                "구매불가"
            );

            return;
        }


        // =============================================
        // 구매 가능
        // =============================================

        purchaseButton.interactable =
            true;

        SetPurchaseButtonText(
            "구매하기"
        );
    }


    private void SetPurchaseButtonText(
        string text
    )
    {
        if (purchaseButtonText != null)
        {
            purchaseButtonText.text =
                text;
        }
    }


    // =========================================================
    // Purchase
    // =========================================================

    private void PurchaseSelectedSkill()
    {
        if (selectedNode == null)
            return;


        bool success =
            selectedNode.Purchase();


        if (!success)
        {
            RefreshPurchaseButton();
            return;
        }


        Debug.Log(
            $"{selectedNode.skillName} 구매 완료"
        );


        // 모든 노드 상태 다시 갱신
        RefreshAllNodes();


        // 현재 설명창 갱신
        RefreshDetailPanel();
    }


    // =========================================================
    // Refresh All
    // =========================================================

    public void RefreshAllNodes()
    {
        SkillNodeUI[] nodes =
            GetComponentsInChildren<SkillNodeUI>(
                true
            );


        foreach (SkillNodeUI node in nodes)
        {
            node.RefreshPurchasedVisual();
        }
    }


    // =========================================================
    // Close Detail
    // =========================================================

    public void CloseDetailPanel()
    {
        selectedNode = null;


        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }
    }
}