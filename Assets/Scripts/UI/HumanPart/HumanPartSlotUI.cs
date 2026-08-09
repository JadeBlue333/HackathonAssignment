using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanPartSlotUI : MonoBehaviour
{
    // =========================================================
    // Human Part Type
    // =========================================================

    public enum HumanPartType
    {
        Head,
        Body,
        Heart
    }


    // =========================================================
    // Display Mode
    // =========================================================

    public enum DisplayMode
    {
        OwnedOnly,
        InventoryDetail
    }


    // =========================================================
    // Part Data
    // =========================================================

    [Header("Part Data")]

    [SerializeField]
    private HumanPartType partType;

    [SerializeField]
    private string partName;

    [TextArea(3, 8)]
    [SerializeField]
    private string description;


    // =========================================================
    // Display Mode
    // =========================================================

    [Header("Display Mode")]

    [SerializeField]
    private DisplayMode displayMode =
        DisplayMode.OwnedOnly;


    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [Tooltip("이 슬롯의 Button")]
    [SerializeField]
    private Button slotButton;

    [Tooltip("파츠를 보유했을 때만 나타나는 실제 이미지")]
    [SerializeField]
    private GameObject ownedVisual;


    // =========================================================
    // Slot Name UI
    // =========================================================

    [Header("Slot Name UI")]

    [Tooltip("이 파츠 슬롯에 따로 표시할 상품명 텍스트")]
    [SerializeField]
    private TMP_Text ownedNameText;

    [Tooltip("미보유 상태에서 표시할 이름")]
    [SerializeField]
    private string unknownName = "????";


    // =========================================================
    // Public Properties
    // =========================================================

    public HumanPartType PartType => partType;

    public string PartName => partName;

    public string Description => description;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (slotButton == null)
        {
            slotButton =
                GetComponent<Button>();
        }

        if (slotButton != null)
        {
            slotButton.onClick.AddListener(
                OnSlotClicked
            );
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        Refresh();
    }


    // =========================================================
    // OnEnable
    // =========================================================

    private void OnEnable()
    {
        Refresh();
    }


    // =========================================================
    // OnDestroy
    // =========================================================

    private void OnDestroy()
    {
        if (slotButton != null)
        {
            slotButton.onClick.RemoveListener(
                OnSlotClicked
            );
        }
    }


    // =========================================================
    // Owned Check
    // =========================================================

    public bool IsOwned()
    {
        if (PlayerStatus.Instance == null)
            return false;


        switch (partType)
        {
            case HumanPartType.Head:

                return PlayerStatus.Instance
                    .humanHead;


            case HumanPartType.Body:

                return PlayerStatus.Instance
                    .humanBody;


            case HumanPartType.Heart:

                return PlayerStatus.Instance
                    .humanHeart;
        }


        return false;
    }


    // =========================================================
    // Part Type Text
    // =========================================================

    public string GetPartTypeText()
    {
        switch (partType)
        {
            case HumanPartType.Head:
                return "HEAD";

            case HumanPartType.Body:
                return "BODY";

            case HumanPartType.Heart:
                return "HEART";
        }


        return "UNKNOWN";
    }


    // =========================================================
    // Refresh
    // =========================================================

    public void Refresh()
    {
        bool owned =
            IsOwned();


        // =====================================================
        // 파츠 이미지
        // =====================================================

        if (ownedVisual != null)
        {
            ownedVisual.SetActive(
                owned
            );
        }


        // =====================================================
        // 슬롯의 별도 상품명 텍스트
        //
        // 미보유 → ????
        // 보유   → 실제 상품명
        // =====================================================

        if (ownedNameText != null)
        {
            ownedNameText.text =
                owned
                ? partName
                : unknownName;
        }


        // =====================================================
        // Button
        // =====================================================

        if (slotButton != null)
        {
            switch (displayMode)
            {
                case DisplayMode.OwnedOnly:

                    slotButton.interactable =
                        owned;

                    break;


                case DisplayMode.InventoryDetail:

                    slotButton.interactable =
                        true;

                    break;
            }
        }
    }


    // =========================================================
    // Click
    // =========================================================

    private void OnSlotClicked()
    {
        if (HumanPartInventoryUI.Instance == null)
            return;


        // =====================================================
        // 기존 씬
        // =====================================================

        if (displayMode ==
            DisplayMode.OwnedOnly)
        {
            if (!IsOwned())
                return;


            HumanPartInventoryUI.Instance.ShowPart(
                partName,
                description
            );


            return;
        }


        // =====================================================
        // 새로운 인벤토리
        // =====================================================

        if (displayMode ==
            DisplayMode.InventoryDetail)
        {
            if (IsOwned())
            {
                HumanPartInventoryUI.Instance.ShowOwnedPart(
                    partName,
                    GetPartTypeText(),
                    description
                );
            }
            else
            {
                HumanPartInventoryUI.Instance
                    .ShowUnknownPart();
            }
        }
    }
}