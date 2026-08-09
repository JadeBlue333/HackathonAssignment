using TMPro;
using UnityEngine;

public class HumanPartInventoryUI : MonoBehaviour
{
    public static HumanPartInventoryUI Instance
    {
        get;
        private set;
    }


    // =========================================================
    // Detail UI
    // =========================================================

    [Header("Detail UI")]

    [Tooltip("설명창 전체 오브젝트")]
    [SerializeField]
    private GameObject detailPanel;

    [Tooltip("아이템 이름")]
    [SerializeField]
    private TMP_Text partNameText;

    [Tooltip("PART // 타입")]
    [SerializeField]
    private TMP_Text partTypeText;

    [Tooltip("아이템 설명")]
    [SerializeField]
    private TMP_Text descriptionText;

    [Tooltip("STATUS : 상태")]
    [SerializeField]
    private TMP_Text statusText;


    // =========================================================
    // Unknown State
    // =========================================================

    [Header("Unknown State")]

    [SerializeField]
    private string unknownName =
        "????";

    [SerializeField]
    private string unknownType =
        "PART // UNKNOWN";

    [TextArea(3, 6)]
    [SerializeField]
    private string unknownDescription =
        "아직 확인되지 않은 부품입니다.\n" +
        "해당 부품을 획득하면 정보를 확인할 수 있습니다.";

    [SerializeField]
    private string unknownStatus =
        "STATUS : 미보유";


    // =========================================================
    // Owned State
    // =========================================================

    [Header("Owned State")]

    [SerializeField]
    private string ownedStatus =
        "STATUS : 보유 중";


    // =========================================================
    // Slots
    // =========================================================

    [Header("Human Part Slots")]

    [SerializeField]
    private HumanPartSlotUI headSlot;

    [SerializeField]
    private HumanPartSlotUI bodySlot;

    [SerializeField]
    private HumanPartSlotUI heartSlot;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // OnDestroy
    // =========================================================

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        RefreshAll();
    }


    // =========================================================
    // OnEnable
    // =========================================================

    private void OnEnable()
    {
        RefreshAll();
    }


    // =========================================================
    // Refresh
    // =========================================================

    public void RefreshAll()
    {
        if (headSlot != null)
        {
            headSlot.Refresh();
        }

        if (bodySlot != null)
        {
            bodySlot.Refresh();
        }

        if (heartSlot != null)
        {
            heartSlot.Refresh();
        }
    }


    // =========================================================
    // 설명창 활성화
    // =========================================================

    private void OpenDetailPanel()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(true);
        }
    }


    // =========================================================
    // 기존 방식
    // =========================================================

    public void ShowPart(
        string partName,
        string description
    )
    {
        OpenDetailPanel();

        if (partNameText != null)
        {
            partNameText.text =
                partName;
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                description;
        }

        if (partTypeText != null)
        {
            partTypeText.text = "";
        }

        if (statusText != null)
        {
            statusText.text = "";
        }
    }


    // =========================================================
    // 보유 파츠
    // =========================================================

    public void ShowOwnedPart(
        string partName,
        string partType,
        string description
    )
    {
        OpenDetailPanel();

        if (partNameText != null)
        {
            partNameText.text =
                partName;
        }

        if (partTypeText != null)
        {
            partTypeText.text =
                $"PART // {partType}";
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                description;
        }

        if (statusText != null)
        {
            statusText.text =
                ownedStatus;
        }
    }


    // =========================================================
    // 미보유 파츠
    // =========================================================

    public void ShowUnknownPart()
    {
        OpenDetailPanel();

        if (partNameText != null)
        {
            partNameText.text =
                unknownName;
        }

        if (partTypeText != null)
        {
            partTypeText.text =
                unknownType;
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                unknownDescription;
        }

        if (statusText != null)
        {
            statusText.text =
                unknownStatus;
        }
    }
}