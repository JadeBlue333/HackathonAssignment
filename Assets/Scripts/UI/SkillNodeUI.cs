using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour
{
    // =========================================================
    // Skill Type
    // =========================================================

    public enum SkillType
    {
        FuelRecovery,          // 자가 충전 회로
        TrustRecovery,         // 평판 보정 모듈
        HighRiskHighReturn,    // 과부하 계약
        WorkTime               // 작업 연장 모듈
    }


    // =========================================================
    // Skill Data
    // =========================================================

    [Header("Skill Data")]

    [SerializeField]
    private SkillType skillType;

    [Tooltip("해당 스킬의 단계. I = 1, II = 2, III = 3")]
    [Min(1)]
    [SerializeField]
    private int level = 1;

    [SerializeField]
    private string skillName;

    [TextArea(3, 8)]
    [SerializeField]
    private string description;

    [SerializeField]
    private int price = 1400;


    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]

    [Tooltip("이 슬롯을 클릭하기 위한 Button")]
    [SerializeField]
    private Button nodeButton;

    [Tooltip("구매 완료 후 활성화할 이미지 오브젝트")]
    [SerializeField]
    private GameObject purchasedTargetImage;

    [Tooltip("이 스킬의 가격을 표시할 TMP 텍스트")]
    [SerializeField]
    private TMP_Text priceText;


    // =========================================================
    // Price Display
    // =========================================================

    [Header("Price Display")]

    [Tooltip("가격 뒤에 붙일 문자열")]
    [SerializeField]
    private string priceSuffix = " C";


    // =========================================================
    // Public Properties
    // =========================================================

    public SkillType Type => skillType;

    public int Level => level;

    public string SkillName => skillName;

    public string Description => description;

    public int Price => price;


    // =========================================================
    // Awake
    // =========================================================

    private void Awake()
    {
        if (nodeButton == null)
        {
            nodeButton = GetComponent<Button>();
        }

        if (nodeButton != null)
        {
            nodeButton.onClick.AddListener(
                OnNodeClicked
            );
        }
    }


    // =========================================================
    // OnDestroy
    // =========================================================

    private void OnDestroy()
    {
        if (nodeButton != null)
        {
            nodeButton.onClick.RemoveListener(
                OnNodeClicked
            );
        }
    }


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        RefreshPriceText();

        RefreshPurchasedImage();
    }


    // =========================================================
    // Node Click
    // =========================================================

    private void OnNodeClicked()
    {
        if (SkillTreeUIManager.Instance == null)
            return;

        SkillTreeUIManager.Instance.SelectNode(
            this
        );
    }


    // =========================================================
    // Price UI
    // =========================================================

    public void RefreshPriceText()
    {
        if (priceText == null)
            return;

        priceText.text =
            $"{price}{priceSuffix}";
    }


    // =========================================================
    // Purchased Check
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
    // Previous Level Check
    // =========================================================

    public bool IsPreviousLevelPurchased()
    {
        if (level <= 1)
            return true;


        if (PlayerStatus.Instance == null)
            return false;


        int requiredLevel =
            level - 1;


        switch (skillType)
        {
            case SkillType.FuelRecovery:

                return PlayerStatus.Instance
                    .fuelRecoveryLevel >= requiredLevel;


            case SkillType.TrustRecovery:

                return PlayerStatus.Instance
                    .trustRecoveryLevel >= requiredLevel;


            case SkillType.HighRiskHighReturn:

                return true;


            case SkillType.WorkTime:

                return PlayerStatus.Instance
                    .workTimeLevel >= requiredLevel;
        }


        return false;
    }


    // =========================================================
    // Money Check
    // =========================================================

    public bool HasEnoughMoney()
    {
        if (PlayerStatus.Instance == null)
            return false;

        return PlayerStatus.Instance.money >= price;
    }


    // =========================================================
    // Purchase Available
    // =========================================================

    public bool CanPurchase()
    {
        if (PlayerStatus.Instance == null)
            return false;


        if (IsPurchased())
            return false;


        if (!IsPreviousLevelPurchased())
            return false;


        if (!HasEnoughMoney())
            return false;


        return true;
    }


    // =========================================================
    // Purchase
    // =========================================================

    public bool Purchase()
    {
        if (!CanPurchase())
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
            RefreshPurchasedImage();

            Debug.Log(
                $"스킬 구매 완료 : {skillName}"
            );
        }


        return success;
    }


    // =========================================================
    // Purchased Visual
    // =========================================================

    public void RefreshPurchasedImage()
    {
        if (purchasedTargetImage == null)
            return;


        purchasedTargetImage.SetActive(
            IsPurchased()
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        price =
            Mathf.Max(
                0,
                price
            );


        level =
            Mathf.Max(
                1,
                level
            );


        RefreshPriceText();
    }
}