using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopPurchaseController : MonoBehaviour
{
    // =========================================================
    // 돈으로 구매하는 아이템
    // =========================================================

    [System.Serializable]
    public class MoneyItem
    {
        [Header("구매 버튼")]
        public Button button;

        [Header("가격")]
        [Tooltip("인덱스 0번 아이템은 다른 스크립트에서 별도로 처리됩니다.")]
        public int price = 100;

        [Header("Hover 안내문")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }


    // =========================================================
    // 연료 관련 아이템
    // =========================================================

    [System.Serializable]
    public class FuelItem
    {
        [Header("버튼")]
        public Button button;

        [Header("연료 소모량")]
        public int fuelCost = 10;

        [Header("Hover 안내문")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }


    // =========================================================
    // Player 정보 UI
    // =========================================================

    [Header("Player 정보")]
    public TextMeshProUGUI playerStat;
    public TextMeshProUGUI playerStat2;


    // =========================================================
    // 돈으로 구매하는 아이템
    // =========================================================

    [Header("돈으로 구매하는 아이템 6개")]
    [SerializeField]
    private List<MoneyItem> moneyItems = new List<MoneyItem>();


    // =========================================================
    // 인간 파츠 Sold Out
    // =========================================================

    [Header("인간 파츠 Sold Out 표시")]
    [SerializeField] private GameObject humanBodySoldOut;
    [SerializeField] private GameObject humanHeadSoldOut;
    [SerializeField] private GameObject humanHeartSoldOut;


    // =========================================================
    // 연료 아이템
    // =========================================================

    [Header("연료 관련 아이템")]
    [SerializeField]
    private FuelItem fuelItem;


    // =========================================================
    // 구매 효과음
    // =========================================================

    [Header("구매 효과음")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip purchaseSfx;

    [Range(0f, 1f)]
    [SerializeField]
    private float purchaseSfxVolume = 1f;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        SetupButtons();
        SetupHoverEvents();
        HideAllNotices();
    }


    // =========================================================
    // Update
    // =========================================================

    private void Update()
    {
        // PlayerStatus가 아직 생성되지 않았다면 실행하지 않음
        if (PlayerStatus.Instance == null)
            return;


        // =====================================================
        // 플레이어 현재 정보 표시
        // =====================================================

        string statusText =
            $"소지금: {PlayerStatus.Instance.money} C  연료: {PlayerStatus.Instance.fuel} / 100";

        if (playerStat != null)
        {
            playerStat.text = statusText;
        }

        if (playerStat2 != null)
        {
            playerStat2.text = statusText;
        }


        // =====================================================
        // 인간 파츠 구매 여부 확인
        // =====================================================

        // -----------------------------------------------------
        // 몸 - moneyItems[3]
        // -----------------------------------------------------

        if (moneyItems.Count > 3)
        {
            bool soldOut = PlayerStatus.Instance.humanBody;

            if (moneyItems[3].button != null)
            {
                moneyItems[3].button.interactable = !soldOut;
            }

            if (humanBodySoldOut != null)
            {
                humanBodySoldOut.SetActive(soldOut);
            }
        }


        // -----------------------------------------------------
        // 머리 - moneyItems[4]
        // -----------------------------------------------------

        if (moneyItems.Count > 4)
        {
            bool soldOut = PlayerStatus.Instance.humanHead;

            if (moneyItems[4].button != null)
            {
                moneyItems[4].button.interactable = !soldOut;
            }

            if (humanHeadSoldOut != null)
            {
                humanHeadSoldOut.SetActive(soldOut);
            }
        }


        // -----------------------------------------------------
        // 심장 - moneyItems[5]
        // -----------------------------------------------------

        if (moneyItems.Count > 5)
        {
            bool soldOut = PlayerStatus.Instance.humanHeart;

            if (moneyItems[5].button != null)
            {
                moneyItems[5].button.interactable = !soldOut;
            }

            if (humanHeartSoldOut != null)
            {
                humanHeartSoldOut.SetActive(soldOut);
            }
        }
    }


    // =========================================================
    // 버튼 자동 연결
    // =========================================================

    private void SetupButtons()
    {
        // -----------------------------------------------------
        // 돈으로 구매하는 아이템 버튼
        // -----------------------------------------------------

        for (int i = 0; i < moneyItems.Count; i++)
        {
            int index = i;

            if (moneyItems[index].button != null)
            {
                moneyItems[index].button.onClick.AddListener(
                    () => PurchaseMoneyItem(index)
                );
            }
        }


        // -----------------------------------------------------
        // 연료 아이템 버튼
        // -----------------------------------------------------

        if (fuelItem != null && fuelItem.button != null)
        {
            fuelItem.button.onClick.AddListener(PurchaseFuelItem);
        }
    }


    // =========================================================
    // Hover 이벤트 자동 연결
    // =========================================================

    private void SetupHoverEvents()
    {
        // -----------------------------------------------------
        // 돈으로 구매하는 아이템
        // -----------------------------------------------------

        for (int i = 0; i < moneyItems.Count; i++)
        {
            int index = i;

            if (moneyItems[index].button == null)
                continue;


            EventTrigger trigger =
                moneyItems[index].button.gameObject.GetComponent<EventTrigger>();


            // EventTrigger가 없다면 자동으로 추가
            if (trigger == null)
            {
                trigger =
                    moneyItems[index].button.gameObject.AddComponent<EventTrigger>();
            }


            if (trigger.triggers == null)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }


            // =================================================
            // Pointer Enter
            // =================================================

            EventTrigger.Entry enterEntry =
                new EventTrigger.Entry();

            enterEntry.eventID =
                EventTriggerType.PointerEnter;

            enterEntry.callback.AddListener(
                (data) => OnMoneyItemHoverEnter(index)
            );

            trigger.triggers.Add(enterEntry);


            // =================================================
            // Pointer Exit
            // =================================================

            EventTrigger.Entry exitEntry =
                new EventTrigger.Entry();

            exitEntry.eventID =
                EventTriggerType.PointerExit;

            exitEntry.callback.AddListener(
                (data) => OnMoneyItemHoverExit(index)
            );

            trigger.triggers.Add(exitEntry);
        }


        // -----------------------------------------------------
        // 연료 아이템
        // -----------------------------------------------------

        if (fuelItem != null && fuelItem.button != null)
        {
            EventTrigger trigger =
                fuelItem.button.gameObject.GetComponent<EventTrigger>();


            if (trigger == null)
            {
                trigger =
                    fuelItem.button.gameObject.AddComponent<EventTrigger>();
            }


            if (trigger.triggers == null)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }


            // =================================================
            // Pointer Enter
            // =================================================

            EventTrigger.Entry enterEntry =
                new EventTrigger.Entry();

            enterEntry.eventID =
                EventTriggerType.PointerEnter;

            enterEntry.callback.AddListener(
                (data) => OnFuelItemHoverEnter()
            );

            trigger.triggers.Add(enterEntry);


            // =================================================
            // Pointer Exit
            // =================================================

            EventTrigger.Entry exitEntry =
                new EventTrigger.Entry();

            exitEntry.eventID =
                EventTriggerType.PointerExit;

            exitEntry.callback.AddListener(
                (data) => OnFuelItemHoverExit()
            );

            trigger.triggers.Add(exitEntry);
        }
    }


    // =========================================================
    // 돈 아이템 구매
    // =========================================================

    public void PurchaseMoneyItem(int index)
    {
        // 잘못된 인덱스 방지
        if (index < 0 || index >= moneyItems.Count)
            return;


        // -----------------------------------------------------
        // 인덱스 0번은 다른 스크립트에서 별도로 처리
        // -----------------------------------------------------

        if (index == 0)
        {
            PlayPurchaseSfx();
            return;
        }


        MoneyItem item = moneyItems[index];


        // -----------------------------------------------------
        // 돈이 부족하면 구매하지 않음
        // -----------------------------------------------------

        if (PlayerStatus.Instance.money < item.price)
            return;


        // -----------------------------------------------------
        // 인간 파츠 구매 처리
        // -----------------------------------------------------

        if (index == 3)
        {
            // 인간 몸
            PlayerStatus.Instance.ObtainHumanBody();
        }
        else if (index == 4)
        {
            // 인간 머리
            PlayerStatus.Instance.ObtainHumanHead();
        }
        else if (index == 5)
        {
            // 인간 심장
            PlayerStatus.Instance.ObtainHumanHeart();
        }


        // -----------------------------------------------------
        // 돈 차감
        // -----------------------------------------------------

        PlayerStatus.Instance.SpendMoney(item.price);


        // -----------------------------------------------------
        // 구매 효과음
        // -----------------------------------------------------

        PlayPurchaseSfx();
    }


    // =========================================================
    // 연료 아이템 처리
    // =========================================================

    public void PurchaseFuelItem()
    {
        if (fuelItem == null)
            return;


        if (PlayerStatus.Instance == null)
            return;


        // -----------------------------------------------------
        // 연료가 부족하면 실행하지 않음
        // -----------------------------------------------------

        if (PlayerStatus.Instance.fuel <= fuelItem.fuelCost)
            return;


        // -----------------------------------------------------
        // 연료 차감
        // -----------------------------------------------------

        PlayerStatus.Instance.AddFuel(-fuelItem.fuelCost);


        // -----------------------------------------------------
        // 연료를 크레딧으로 환전
        // 1 연료 = 2 크레딧
        // -----------------------------------------------------

        PlayerStatus.Instance.AddMoney(
            fuelItem.fuelCost * 2
        );


        // -----------------------------------------------------
        // 구매 효과음
        // -----------------------------------------------------

        PlayPurchaseSfx();
    }


    // =========================================================
    // 돈 아이템 Hover Enter
    // =========================================================

    private void OnMoneyItemHoverEnter(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;


        if (PlayerStatus.Instance == null)
            return;


        MoneyItem item = moneyItems[index];


        // -----------------------------------------------------
        // 일단 안내문 전부 숨기기
        // -----------------------------------------------------

        if (item.canPurchaseNotice != null)
        {
            item.canPurchaseNotice.SetActive(false);
        }

        if (item.cannotPurchaseNotice != null)
        {
            item.cannotPurchaseNotice.SetActive(false);
        }


        // -----------------------------------------------------
        // 인덱스 0번 특별 처리
        // 다른 스크립트에서 구매를 처리하므로
        // 현재 돈이 있는지만 확인
        // -----------------------------------------------------

        if (index == 0)
        {
            if (PlayerStatus.Instance.money > 0)
            {
                // 구매 가능
                if (item.canPurchaseNotice != null)
                {
                    item.canPurchaseNotice.SetActive(true);
                }
            }
            else
            {
                // 구매 불가능
                if (item.cannotPurchaseNotice != null)
                {
                    item.cannotPurchaseNotice.SetActive(true);
                }
            }

            return;
        }


        // -----------------------------------------------------
        // 인덱스 1~5 일반 구매
        // -----------------------------------------------------

        if (PlayerStatus.Instance.money >= item.price)
        {
            // 구매 가능
            if (item.canPurchaseNotice != null)
            {
                item.canPurchaseNotice.SetActive(true);
            }
        }
        else
        {
            // 구매 불가능
            if (item.cannotPurchaseNotice != null)
            {
                item.cannotPurchaseNotice.SetActive(true);
            }
        }
    }


    // =========================================================
    // 돈 아이템 Hover Exit
    // =========================================================

    private void OnMoneyItemHoverExit(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;


        MoneyItem item = moneyItems[index];


        if (item.canPurchaseNotice != null)
        {
            item.canPurchaseNotice.SetActive(false);
        }

        if (item.cannotPurchaseNotice != null)
        {
            item.cannotPurchaseNotice.SetActive(false);
        }
    }


    // =========================================================
    // 연료 아이템 Hover Enter
    // =========================================================

    private void OnFuelItemHoverEnter()
    {
        if (fuelItem == null)
            return;


        if (PlayerStatus.Instance == null)
            return;


        // -----------------------------------------------------
        // 일단 안내문 전부 숨기기
        // -----------------------------------------------------

        if (fuelItem.canPurchaseNotice != null)
        {
            fuelItem.canPurchaseNotice.SetActive(false);
        }

        if (fuelItem.cannotPurchaseNotice != null)
        {
            fuelItem.cannotPurchaseNotice.SetActive(false);
        }


        // -----------------------------------------------------
        // 연료 보유량 확인
        // -----------------------------------------------------

        if (PlayerStatus.Instance.fuel >= fuelItem.fuelCost)
        {
            // 가능
            if (fuelItem.canPurchaseNotice != null)
            {
                fuelItem.canPurchaseNotice.SetActive(true);
            }
        }
        else
        {
            // 불가능
            if (fuelItem.cannotPurchaseNotice != null)
            {
                fuelItem.cannotPurchaseNotice.SetActive(true);
            }
        }
    }


    // =========================================================
    // 연료 아이템 Hover Exit
    // =========================================================

    private void OnFuelItemHoverExit()
    {
        if (fuelItem == null)
            return;


        if (fuelItem.canPurchaseNotice != null)
        {
            fuelItem.canPurchaseNotice.SetActive(false);
        }

        if (fuelItem.cannotPurchaseNotice != null)
        {
            fuelItem.cannotPurchaseNotice.SetActive(false);
        }
    }


    // =========================================================
    // 모든 Hover 안내문 숨기기
    // =========================================================

    private void HideAllNotices()
    {
        foreach (MoneyItem item in moneyItems)
        {
            if (item.canPurchaseNotice != null)
            {
                item.canPurchaseNotice.SetActive(false);
            }

            if (item.cannotPurchaseNotice != null)
            {
                item.cannotPurchaseNotice.SetActive(false);
            }
        }


        if (fuelItem != null)
        {
            if (fuelItem.canPurchaseNotice != null)
            {
                fuelItem.canPurchaseNotice.SetActive(false);
            }

            if (fuelItem.cannotPurchaseNotice != null)
            {
                fuelItem.cannotPurchaseNotice.SetActive(false);
            }
        }
    }


    // =========================================================
    // 구매 효과음 재생
    // =========================================================

    private void PlayPurchaseSfx()
    {
        if (audioSource != null && purchaseSfx != null)
        {
            audioSource.PlayOneShot(
                purchaseSfx,
                purchaseSfxVolume
            );
        }
    }
}