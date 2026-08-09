using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopPurchaseController : MonoBehaviour
{
    [System.Serializable]
    public class MoneyItem
    {
        [Header("구매 버튼")]
        public Button button;

        [Header("가격. 잭팟은 우선 0으로 처리 후 잭팟 컨트롤러에서 관리")]
        public int price = 100;

        [Header("Hover 안내문")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }

    [System.Serializable]
    public class FuelItem
    {
        [Header("구매 버튼")]
        public Button button;

        [Header("기름 소비량")]
        public int fuelCost = 10;

        [Header("Hover 안내문")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }

    [Header("Player 상태")]
    public TextMeshProUGUI playerStat;
    public TextMeshProUGUI playerStat2;

    [Header("돈으로 구매하는 아이템 6개")]
    [SerializeField]
    private List<MoneyItem> moneyItems = new List<MoneyItem>();

    [Header("기름으로 구매하는 아이템")]
    [SerializeField]
    private FuelItem fuelItem;

    [Header("공통 구매 효과음")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip purchaseSfx;

    [Range(0f, 1f)]
    [SerializeField]
    private float purchaseSfxVolume = 1f;


    private void Start()
    {
        SetupButtons();
        SetupHoverEvents();
        HideAllNotices();
    }

    private void Update()
    {
        playerStat.text = $"가진 돈: {PlayerStatus.Instance.money} C  연료: {PlayerStatus.Instance.fuel} / 100";
        playerStat2.text = $"가진 돈: {PlayerStatus.Instance.money} C  연료: {PlayerStatus.Instance.fuel} / 100";
    }


    // =========================================================
    // 버튼 자동 연결
    // =========================================================

    private void SetupButtons()
    {
        // 돈 구매 버튼 자동 연결
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

        // 기름 구매 버튼 자동 연결
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
        // 돈 구매 버튼
        for (int i = 0; i < moneyItems.Count; i++)
        {
            int index = i;

            if (moneyItems[index].button == null)
                continue;

            EventTrigger trigger =
                moneyItems[index].button.gameObject.GetComponent<EventTrigger>();

            if (trigger == null)
            {
                trigger = moneyItems[index].button.gameObject.AddComponent<EventTrigger>();
            }

            if (trigger.triggers == null)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }


            // Pointer Enter
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;

            enterEntry.callback.AddListener(
                (data) => OnMoneyItemHoverEnter(index)
            );

            trigger.triggers.Add(enterEntry);


            // Pointer Exit
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;

            exitEntry.callback.AddListener(
                (data) => OnMoneyItemHoverExit(index)
            );

            trigger.triggers.Add(exitEntry);
        }


        // 기름 구매 버튼
        if (fuelItem != null && fuelItem.button != null)
        {
            EventTrigger trigger =
                fuelItem.button.gameObject.GetComponent<EventTrigger>();

            if (trigger == null)
            {
                trigger = fuelItem.button.gameObject.AddComponent<EventTrigger>();
            }

            if (trigger.triggers == null)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }


            // Pointer Enter
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;

            enterEntry.callback.AddListener(
                (data) => OnFuelItemHoverEnter()
            );

            trigger.triggers.Add(enterEntry);


            // Pointer Exit
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;

            exitEntry.callback.AddListener(
                (data) => OnFuelItemHoverExit()
            );

            trigger.triggers.Add(exitEntry);
        }
    }


    // =========================================================
    // 돈으로 아이템 구매
    // =========================================================

    // =========================================================
    // 돈으로 아이템 구매
    // =========================================================

    public void PurchaseMoneyItem(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;

        // 인덱스 0은 다른 스크립트에서 결제 처리
        if (index == 0)
        {
            PlayPurchaseSfx();
            return;
        }

        MoneyItem item = moneyItems[index];

        // 돈 부족하면 아무것도 하지 않음
        if (PlayerStatus.Instance.money < item.price)
            return;

        // 돈 차감
        PlayerStatus.Instance.SpendMoney(item.price);

        // 공통 구매 효과음
        PlayPurchaseSfx();
    }


    // =========================================================
    // 기름으로 아이템 구매
    // =========================================================

    public void PurchaseFuelItem()
    {
        if (fuelItem == null)
            return;

        // 기름 부족하면 아무것도 하지 않음
        if (PlayerStatus.Instance.fuel < fuelItem.fuelCost)
            return;

        // 기름 차감
        PlayerStatus.Instance.AddFuel(-fuelItem.fuelCost);

        // 기름 → 크레타 환전
        // 1 기름 = 1/2 크레타
        PlayerStatus.Instance.AddMoney(fuelItem.fuelCost / 2);

        // 공통 구매 효과음
        PlayPurchaseSfx();
    }


    // =========================================================
    // 돈 아이템 Hover
    // =========================================================

    private void OnMoneyItemHoverEnter(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;

        MoneyItem item = moneyItems[index];

        // 먼저 둘 다 끄기
        if (item.canPurchaseNotice != null)
            item.canPurchaseNotice.SetActive(false);

        if (item.cannotPurchaseNotice != null)
            item.cannotPurchaseNotice.SetActive(false);


        // ---------------------------------------------------------
        // 인덱스 0은 특별 처리
        // 다른 스크립트에서 결제하므로
        // "돈이 0원인가?"만 확인
        // ---------------------------------------------------------

        if (index == 0)
        {
            if (PlayerStatus.Instance.money > 0)
            {
                // 돈이 있으면 살 수 있음
                if (item.canPurchaseNotice != null)
                    item.canPurchaseNotice.SetActive(true);
            }
            else
            {
                // 돈이 0원이면 살 수 없음
                if (item.cannotPurchaseNotice != null)
                    item.cannotPurchaseNotice.SetActive(true);
            }

            return;
        }


        // ---------------------------------------------------------
        // 1~5번은 기존 방식
        // ---------------------------------------------------------

        if (PlayerStatus.Instance.money >= item.price)
        {
            // 돈 충분
            if (item.canPurchaseNotice != null)
                item.canPurchaseNotice.SetActive(true);
        }
        else
        {
            // 돈 부족
            if (item.cannotPurchaseNotice != null)
                item.cannotPurchaseNotice.SetActive(true);
        }
    }


    private void OnMoneyItemHoverExit(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;

        MoneyItem item = moneyItems[index];

        if (item.canPurchaseNotice != null)
            item.canPurchaseNotice.SetActive(false);

        if (item.cannotPurchaseNotice != null)
            item.cannotPurchaseNotice.SetActive(false);
    }

    // =========================================================
    // 기름 아이템 Hover
    // =========================================================

    private void OnFuelItemHoverEnter()
    {
        if (fuelItem == null)
            return;

        // 먼저 둘 다 끄기
        if (fuelItem.canPurchaseNotice != null)
            fuelItem.canPurchaseNotice.SetActive(false);

        if (fuelItem.cannotPurchaseNotice != null)
            fuelItem.cannotPurchaseNotice.SetActive(false);


        // 기름 충분
        if (PlayerStatus.Instance.fuel >= fuelItem.fuelCost)
        {
            if (fuelItem.canPurchaseNotice != null)
                fuelItem.canPurchaseNotice.SetActive(true);
        }
        // 기름 부족
        else
        {
            if (fuelItem.cannotPurchaseNotice != null)
                fuelItem.cannotPurchaseNotice.SetActive(true);
        }
    }


    private void OnFuelItemHoverExit()
    {
        if (fuelItem == null)
            return;

        if (fuelItem.canPurchaseNotice != null)
            fuelItem.canPurchaseNotice.SetActive(false);

        if (fuelItem.cannotPurchaseNotice != null)
            fuelItem.cannotPurchaseNotice.SetActive(false);
    }


    // =========================================================
    // 모든 안내문 끄기
    // =========================================================

    private void HideAllNotices()
    {
        foreach (MoneyItem item in moneyItems)
        {
            if (item.canPurchaseNotice != null)
                item.canPurchaseNotice.SetActive(false);

            if (item.cannotPurchaseNotice != null)
                item.cannotPurchaseNotice.SetActive(false);
        }

        if (fuelItem != null)
        {
            if (fuelItem.canPurchaseNotice != null)
                fuelItem.canPurchaseNotice.SetActive(false);

            if (fuelItem.cannotPurchaseNotice != null)
                fuelItem.cannotPurchaseNotice.SetActive(false);
        }
    }


    // =========================================================
    // 공통 구매 효과음
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