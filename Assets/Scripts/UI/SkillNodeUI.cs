using UnityEngine;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour
{
    public enum SkillType
    {
        FuelRecovery,
        TrustRecovery,
        ConcealProtocol,
        HighRiskHighReturn,
        WorkTime
    }


    // =========================================================
    // Skill Data
    // =========================================================

    [Header("Skill")]
    public SkillType skillType;

    [Tooltip("이 노드의 단계. 1부터 시작")]
    [Min(1)]
    public int level = 1;


    [Header("Info")]

    public string skillName;

    [TextArea(3, 8)]
    public string description;

    public int price = 1400;


    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [Tooltip("이 스킬을 구매했을 때 켜질 이미지")]
    [SerializeField]
    private GameObject purchasedImage;

    [Tooltip("현재 스킬 노드의 Button")]
    [SerializeField]
    private Button button;


    // =========================================================
    // Start
    // =========================================================

    private void Awake()
    {
        if (button == null)
        {
            button =
                GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(
                OnClickNode
            );
        }
    }


    private void Start()
    {
        RefreshPurchasedVisual();
    }


    // =========================================================
    // Click
    // =========================================================

    private void OnClickNode()
    {
        if (SkillTreeUIManager.Instance == null)
            return;

        SkillTreeUIManager.Instance.SelectNode(
            this
        );
    }


    // =========================================================
    // Purchased
    // =========================================================

    public bool IsPurchased()
    {
        if (PlayerStatus.Instance == null)
            return false;


        switch (skillType)
        {
            case SkillType.FuelRecovery:
                return PlayerStatus.Instance
                           .fuelRecoveryLevel >= level;


            case SkillType.TrustRecovery:
                return PlayerStatus.Instance
                           .trustRecoveryLevel >= level;


            case SkillType.ConcealProtocol:
                return PlayerStatus.Instance
                           .concealItemLevel >= level;


            case SkillType.HighRiskHighReturn:
                return PlayerStatus.Instance
                           .highRiskHighReturnLevel >= level;


            case SkillType.WorkTime:
                return PlayerStatus.Instance
                           .workTimeLevel >= level;
        }


        return false;
    }


    // =========================================================
    // Previous Level
    // =========================================================

    public bool IsPreviousLevelPurchased()
    {
        // Lv.1은 이전 스킬이 없으므로 바로 해금
        if (level <= 1)
            return true;


        if (PlayerStatus.Instance == null)
            return false;


        int previousLevel =
            level - 1;


        switch (skillType)
        {
            case SkillType.FuelRecovery:
                return PlayerStatus.Instance
                           .fuelRecoveryLevel >= previousLevel;


            case SkillType.TrustRecovery:
                return PlayerStatus.Instance
                           .trustRecoveryLevel >= previousLevel;


            case SkillType.ConcealProtocol:
                return PlayerStatus.Instance
                           .concealItemLevel >= previousLevel;


            case SkillType.HighRiskHighReturn:
                return true;


            case SkillType.WorkTime:
                return PlayerStatus.Instance
                           .workTimeLevel >= previousLevel;
        }


        return false;
    }


    // =========================================================
    // Purchase
    // =========================================================

    public bool Purchase()
    {
        if (PlayerStatus.Instance == null)
            return false;


        // 이미 구매
        if (IsPurchased())
            return false;


        // 이전 단계 미구매
        if (!IsPreviousLevelPurchased())
            return false;


        bool success = false;


        switch (skillType)
        {
            case SkillType.FuelRecovery:

                success =
                    PlayerStatus.Instance
                        .PurchaseFuelRecovery(
                            price
                        );

                break;


            case SkillType.TrustRecovery:

                success =
                    PlayerStatus.Instance
                        .PurchaseTrustRecovery(
                            price
                        );

                break;


            case SkillType.ConcealProtocol:

                success =
                    PlayerStatus.Instance
                        .PurchaseConcealItem(
                            price
                        );

                break;


            case SkillType.HighRiskHighReturn:

                success =
                    PlayerStatus.Instance
                        .PurchaseHighRiskHighReturn(
                            price
                        );

                break;


            case SkillType.WorkTime:

                success =
                    PlayerStatus.Instance
                        .PurchaseWorkTime(
                            price
                        );

                break;
        }


        if (success)
        {
            RefreshPurchasedVisual();
        }


        return success;
    }


    // =========================================================
    // Visual
    // =========================================================

    public void RefreshPurchasedVisual()
    {
        if (purchasedImage == null)
            return;


        purchasedImage.SetActive(
            IsPurchased()
        );
    }
}