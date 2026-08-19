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

    [Tooltip("아래쪽 설명/구매창 전체")]
    [SerializeField]
    private GameObject detailPanel;


    // =========================================================
    // Text
    // =========================================================

    [Header("Text")]

    [SerializeField]
    private TMP_Text skillNameText;

    [SerializeField]
    private TMP_Text descriptionText;

    [Tooltip("현재 선택한 스킬의 가격 표시")]
    [SerializeField]
    private TMP_Text selectedPriceText;


    // =========================================================
    // Purchase Button
    // =========================================================

    [Header("Purchase Button")]

    [SerializeField]
    private Button purchaseButton;

    [SerializeField]
    private TMP_Text purchaseButtonText;


    // =========================================================
    // Audio
    // =========================================================

    [Header("Purchase Audio")]

    [SerializeField]
    private AudioSource audioSource;

    [Tooltip("구매 성공 효과음")]
    [SerializeField]
    private AudioClip purchaseSuccessSfx;

    [Range(0f, 1f)]
    [SerializeField]
    private float purchaseSuccessVolume = 1f;


    [Tooltip("구매 불가 효과음")]
    [SerializeField]
    private AudioClip purchaseFailSfx;

    [Range(0f, 1f)]
    [SerializeField]
    private float purchaseFailVolume = 1f;


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
                OnPurchaseButtonClicked
            );
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }

        RefreshAllNodes();
    }


    // =========================================================
    // Select Node
    // =========================================================

    public void SelectNode(
        SkillNodeUI node
    )
    {
        if (node == null)
            return;


        selectedNode = node;


        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
        }


        RefreshDetailPanel();
    }


    // =========================================================
    // Detail Panel
    // =========================================================

    private void RefreshDetailPanel()
    {
        if (selectedNode == null)
            return;


        // 이름
        if (skillNameText != null)
        {
            skillNameText.text =
                selectedNode.SkillName;
        }


        // 설명
        if (descriptionText != null)
        {
            descriptionText.text =
                selectedNode.Description;
        }


        // 가격
        if (selectedPriceText != null)
        {
            selectedPriceText.text =
                $"{selectedNode.Price} C";
        }


        RefreshPurchaseButton();
    }


    // =========================================================
    // Purchase Button State
    // =========================================================

    private void RefreshPurchaseButton()
    {
        if (selectedNode == null)
            return;

        if (purchaseButton == null)
            return;


        purchaseButton.interactable = true;


        // 이미 구매 완료
        if (selectedNode.IsPurchased())
        {
            SetPurchaseButtonText(
                GetLocalizedText(
                    "구매완료",
                    "PURCHASED"
                )
            );

            return;
        }


        // 이전 단계 미구매
        if (!selectedNode.IsPreviousLevelPurchased())
        {
            SetPurchaseButtonText(
                GetLocalizedText(
                    "구매불가",
                    "UNAVAILABLE"
                )
            );

            return;
        }


        // 돈 부족
        if (!selectedNode.HasEnoughMoney())
        {
            SetPurchaseButtonText(
                GetLocalizedText(
                    "구매불가",
                    "UNAVAILABLE"
                )
            );

            return;
        }


        // 구매 가능
        SetPurchaseButtonText(
            GetLocalizedText(
                "구매하기",
                "PURCHASE"
            )
        );
    }


    // =========================================================
    // Purchase Button Click
    // =========================================================

    private void OnPurchaseButtonClicked()
    {
        if (selectedNode == null)
            return;


        // 이미 구매한 스킬
        if (selectedNode.IsPurchased())
        {
            return;
        }


        // 이전 단계가 안 열림
        if (!selectedNode.IsPreviousLevelPurchased())
        {
            PlaySfx(
                purchaseFailSfx,
                purchaseFailVolume
            );

            Debug.Log(
                "구매 불가 - 이전 단계 스킬이 필요합니다."
            );

            return;
        }


        // 돈 부족
        if (!selectedNode.HasEnoughMoney())
        {
            PlaySfx(
                purchaseFailSfx,
                purchaseFailVolume
            );

            Debug.Log(
                "구매 불가 - 돈이 부족합니다."
            );

            return;
        }


        // 구매 시도
        bool success =
            selectedNode.Purchase();


        // 구매 성공
        if (success)
        {
            PlaySfx(
                purchaseSuccessSfx,
                purchaseSuccessVolume
            );


            Debug.Log(
                $"{selectedNode.SkillName} 구매 완료"
            );


            RefreshAllNodes();

            RefreshDetailPanel();

            return;
        }


        // 예상하지 못한 구매 실패
        PlaySfx(
            purchaseFailSfx,
            purchaseFailVolume
        );


        RefreshDetailPanel();
    }


    // =========================================================
    // Localized Text
    // =========================================================

    private string GetLocalizedText(
        string koreanText,
        string englishText
    )
    {
        if (LanguageManager.Instance == null)
        {
            return koreanText;
        }

        return LanguageManager.Instance.isEnglish
            ? englishText
            : koreanText;
    }


    // =========================================================
    // Button Text
    // =========================================================

    private void SetPurchaseButtonText(
        string value
    )
    {
        if (purchaseButtonText != null)
        {
            purchaseButtonText.text =
                value;
        }
    }


    // =========================================================
    // Audio
    // =========================================================

    private void PlaySfx(
        AudioClip clip,
        float volume
    )
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;


        audioSource.PlayOneShot(
            clip,
            volume
        );
    }


    // =========================================================
    // Refresh All
    // =========================================================

    public void RefreshAllNodes()
    {
        SkillNodeUI[] nodes =
            FindObjectsByType<SkillNodeUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (SkillNodeUI node in nodes)
        {
            node.RefreshPurchasedImage();
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