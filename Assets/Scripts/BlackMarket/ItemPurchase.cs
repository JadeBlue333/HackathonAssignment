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
        [Header("���� ��ư")]
        public Button button;

        [Header("����. ������ �켱 0���� ó�� �� ���� ��Ʈ�ѷ����� ����")]
        public int price = 100;

        [Header("Hover �ȳ���")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }

    [System.Serializable]
    public class FuelItem
    {
        [Header("���� ��ư")]
        public Button button;

        [Header("�⸧ �Һ�")]
        public int fuelCost = 10;

        [Header("Hover �ȳ���")]
        public GameObject canPurchaseNotice;
        public GameObject cannotPurchaseNotice;
    }

    [Header("Player ����")]
    public TextMeshProUGUI playerStat;
    public TextMeshProUGUI playerStat2;

    [Header("������ �����ϴ� ������ 6��")]
    [SerializeField]
    private List<MoneyItem> moneyItems = new List<MoneyItem>();

    [Header("�ΰ� ���� Sold Out ǥ��")]
    [SerializeField] private GameObject humanBodySoldOut;
    [SerializeField] private GameObject humanHeadSoldOut;
    [SerializeField] private GameObject humanHeartSoldOut;

    [Header("�⸧���� �����ϴ� ������")]
    [SerializeField]
    private FuelItem fuelItem;

    [Header("���� ���� ȿ����")]
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
        playerStat.text =
            $"���� ��: {PlayerStatus.Instance.money} C  ����: {PlayerStatus.Instance.fuel} / 100";

        playerStat2.text =
            $"���� ��: {PlayerStatus.Instance.money} C  ����: {PlayerStatus.Instance.fuel} / 100";


        // =========================================================
        // �ΰ� ���� ���� ���� Ȯ��
        // =========================================================

        // ���� - moneyItems[3]
        if (moneyItems.Count > 3)
        {
            bool soldOut = PlayerStatus.Instance.humanBody;

            moneyItems[3].button.interactable = !soldOut;

            if (humanBodySoldOut != null)
                humanBodySoldOut.SetActive(soldOut);
        }


        // �Ӹ� - moneyItems[4]
        if (moneyItems.Count > 4)
        {
            bool soldOut = PlayerStatus.Instance.humanHead;

            moneyItems[4].button.interactable = !soldOut;

            if (humanHeadSoldOut != null)
                humanHeadSoldOut.SetActive(soldOut);
        }


        // ���� - moneyItems[5]
        if (moneyItems.Count > 5)
        {
            bool soldOut = PlayerStatus.Instance.humanHeart;

            moneyItems[5].button.interactable = !soldOut;

            if (humanHeartSoldOut != null)
                humanHeartSoldOut.SetActive(soldOut);
        }
    }


    // =========================================================
    // ��ư �ڵ� ����
    // =========================================================

    private void SetupButtons()
    {
        // �� ���� ��ư �ڵ� ����
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

        // �⸧ ���� ��ư �ڵ� ����
        if (fuelItem != null && fuelItem.button != null)
        {
            fuelItem.button.onClick.AddListener(PurchaseFuelItem);
        }
    }


    // =========================================================
    // Hover �̺�Ʈ �ڵ� ����
    // =========================================================

    private void SetupHoverEvents()
    {
        // �� ���� ��ư
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


        // �⸧ ���� ��ư
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
    // ������ ������ ����
    // =========================================================

    // =========================================================
    // ������ ������ ����
    // =========================================================

    public void PurchaseMoneyItem(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;

        // �ε��� 0�� �ٸ� ��ũ��Ʈ���� ���� ó��
        if (index == 0)
        {
            PlayPurchaseSfx();
            return;
        }

        MoneyItem item = moneyItems[index];

        // �� �����ϸ� �ƹ��͵� ���� ����
        if (PlayerStatus.Instance.money < item.price)
            return;

        if (index == 3) // �ΰ� ����
        {
            PlayerStatus.Instance.ObtainHumanBody();
        }
        else if (index == 4) // �ΰ� �Ӹ�
        {
            PlayerStatus.Instance.ObtainHumanHead();
        }
        else if (index == 5) // �ΰ� ����
        {
            PlayerStatus.Instance.ObtainHumanHeart();
        }

        // �� ����
        PlayerStatus.Instance.SpendMoney(item.price);

        // ���� ���� ȿ����
        PlayPurchaseSfx();
    }


    // =========================================================
    // �⸧���� ������ ����
    // =========================================================

    public void PurchaseFuelItem()
    {
        if (fuelItem == null)
            return;

        // �⸧ �����ϸ� �ƹ��͵� ���� ����
        if (PlayerStatus.Instance.fuel <= fuelItem.fuelCost)
            return;

        // �⸧ ����
        PlayerStatus.Instance.AddFuel(-fuelItem.fuelCost);

        // �⸧ �� ũ��Ÿ ȯ��
        // 1 �⸧ = 1/2 ũ��Ÿ
        PlayerStatus.Instance.AddMoney(fuelItem.fuelCost * 2);

        // ���� ���� ȿ����
        PlayPurchaseSfx();
    }


    // =========================================================
    // �� ������ Hover
    // =========================================================

    private void OnMoneyItemHoverEnter(int index)
    {
        if (index < 0 || index >= moneyItems.Count)
            return;

        MoneyItem item = moneyItems[index];

        // ���� �� �� ����
        if (item.canPurchaseNotice != null)
            item.canPurchaseNotice.SetActive(false);

        if (item.cannotPurchaseNotice != null)
            item.cannotPurchaseNotice.SetActive(false);


        // ---------------------------------------------------------
        // �ε��� 0�� Ư�� ó��
        // �ٸ� ��ũ��Ʈ���� �����ϹǷ�
        // "���� 0���ΰ�?"�� Ȯ��
        // ---------------------------------------------------------

        if (index == 0)
        {
            if (PlayerStatus.Instance.money > 0)
            {
                // ���� ������ �� �� ����
                if (item.canPurchaseNotice != null)
                    item.canPurchaseNotice.SetActive(true);
            }
            else
            {
                // ���� 0���̸� �� �� ����
                if (item.cannotPurchaseNotice != null)
                    item.cannotPurchaseNotice.SetActive(true);
            }

            return;
        }


        // ---------------------------------------------------------
        // 1~5���� ���� ���
        // ---------------------------------------------------------

        if (PlayerStatus.Instance.money >= item.price)
        {
            // �� ���
            if (item.canPurchaseNotice != null)
                item.canPurchaseNotice.SetActive(true);
        }
        else
        {
            // �� ����
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
    // �⸧ ������ Hover
    // =========================================================

    private void OnFuelItemHoverEnter()
    {
        if (fuelItem == null)
            return;

        // ���� �� �� ����
        if (fuelItem.canPurchaseNotice != null)
            fuelItem.canPurchaseNotice.SetActive(false);

        if (fuelItem.cannotPurchaseNotice != null)
            fuelItem.cannotPurchaseNotice.SetActive(false);


        // �⸧ ���
        if (PlayerStatus.Instance.fuel >= fuelItem.fuelCost)
        {
            if (fuelItem.canPurchaseNotice != null)
                fuelItem.canPurchaseNotice.SetActive(true);
        }
        // �⸧ ����
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
    // ��� �ȳ��� ����
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
    // ���� ���� ȿ����
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