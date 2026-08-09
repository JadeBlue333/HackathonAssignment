using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }


    // =========================================================
    // Initialize
    // =========================================================

    private void Initialize()
    {
        // 기본 상태
        currentDay = 9;
        money = 20;

        fuel = 70;
        trust = 50;

        earnings = 0;
        trustChange = 0;

        comboNumber = 0;
        mistakeNumber = 0;

        // 인간 파츠 초기화
        humanBody = false;
        humanHead = false;
        humanHeart = false;

        isHuman = false;


        // 스킬 초기화
        fuelRecoveryLevel = 0;
        trustRecoveryLevel = 0;
        highRiskHighReturnLevel = 0;
        workTimeLevel = 0;


        // 기본 작업 시간 적용
        UpdateDayDuration();
    }


    [Header("Inspector에서 수정해도 게임 시작 시 Initialize 값으로 초기화됩니다.")]


    // =========================================================
    // Date
    // =========================================================

    [Header("Date")]
    [Range(0, 9)]
    public int currentDay;


    // =========================================================
    // Money
    // =========================================================

    [Header("Money")]
    public int money;

    public int earnings;


    // =========================================================
    // Fuel
    // =========================================================

    [Header("Fuel")]
    [Range(0, 100)]
    public int fuel;

    public const int MaxFuel = 100;


    // =========================================================
    // Trust
    // =========================================================

    [Header("Trust")]
    [Range(0, 100)]
    public int trust;

    public int trustChange;

    public const int MaxTrust = 100;

    public const int TrustGradeA = 70;
    public const int TrustGradeB = 40;


    // =========================================================
    // Human Parts
    // =========================================================

    [Header("Human Parts")]

    [Tooltip("인간의 몸 파츠 보유 여부")]
    public bool humanBody = false;

    [Tooltip("인간의 머리 파츠 보유 여부")]
    public bool humanHead = false;

    [Tooltip("인간의 심장 파츠 보유 여부")]
    public bool humanHeart = false;

    [Tooltip("인간이 되었는지 여부")]
    public bool isHuman = false;


    // =========================================================
    // Skill Tree - Current Levels
    // =========================================================

    [Header("Skill Tree - Current Level")]

    [Tooltip("연료 자동 회복 Lv.0~3")]
    [Range(0, 3)]
    public int fuelRecoveryLevel = 0;

    [Tooltip("신뢰도 자동 회복 Lv.0~3")]
    [Range(0, 3)]
    public int trustRecoveryLevel = 0;

    [Tooltip("하이리스크 하이리턴 Lv.0~1")]
    [Range(0, 1)]
    public int highRiskHighReturnLevel = 0;

    [Tooltip("작업시간 증가 Lv.0~3")]
    [Range(0, 3)]
    public int workTimeLevel = 0;


    // =========================================================
    // Skill Tree - Fuel Recovery
    // =========================================================

    [Header("Skill - Fuel Recovery")]

    [Tooltip("연료 회복 Lv.1 효과")]
    [SerializeField]
    private int fuelRecoveryLv1 = 5;

    [Tooltip("연료 회복 Lv.2 효과")]
    [SerializeField]
    private int fuelRecoveryLv2 = 10;

    [Tooltip("연료 회복 Lv.3 효과")]
    [SerializeField]
    private int fuelRecoveryLv3 = 15;


    // =========================================================
    // Skill Tree - Trust Recovery
    // =========================================================

    [Header("Skill - Trust Recovery")]

    [Tooltip("신뢰 회복 Lv.1 효과")]
    [SerializeField]
    private int trustRecoveryLv1 = 3;

    [Tooltip("신뢰 회복 Lv.2 효과")]
    [SerializeField]
    private int trustRecoveryLv2 = 6;

    [Tooltip("신뢰 회복 Lv.3 효과")]
    [SerializeField]
    private int trustRecoveryLv3 = 10;


    // =========================================================
    // Skill Tree - Work Time
    // =========================================================

    [Header("Skill - Work Time")]

    [Tooltip("기본 하루 실제 플레이 시간. 180초 = 게임 내 09:00 ~ 15:00")]
    [SerializeField]
    private float baseDayDuration = 180f;

    [Tooltip("작업 연장 모듈 1단계당 실제 추가 시간. 15초 = 게임 내 30분")]
    [SerializeField]
    private float workTimeBonusPerLevel = 15f;


    // =========================================================
    // Time
    // =========================================================

    [Header("Time")]

    [Tooltip("현재 적용된 하루 실제 플레이 시간")]
    public float dayDuration = 180f;


    // =========================================================
    // Combo / Mistake
    // =========================================================

    [Header("콤보 / 실수")]
    public int comboNumber = 0;
    public int mistakeNumber = 0;


    // =========================================================
    // Money Functions
    // =========================================================

    public void AddEarnings(int amount)
    {
        if (IsHighRiskHighReturnActive())
        {
            amount *= 2;
        }

        earnings += amount;
    }


    public void ResetEarnings()
    {
        earnings = 0;
    }


    public void ApplyEarnings()
    {
        AddMoney(earnings);
        ResetEarnings();
    }


    public void AddMoney(int amount)
    {
        money += amount;
    }


    public bool SpendMoney(int amount)
    {
        if (money < amount)
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }

        money -= amount;

        Debug.Log(
            $"{amount} 사용 / 현재 보유 금액: {money}"
        );

        return true;
    }


    // =========================================================
    // Fuel Functions
    // =========================================================

    public void AddFuel(int amount)
    {
        fuel = Mathf.Clamp(
            fuel + amount,
            0,
            MaxFuel
        );
    }


    public void ReduceFuel(int amount)
    {
        fuel = Mathf.Clamp(
            fuel - amount,
            0,
            MaxFuel
        );
    }


    // =========================================================
    // Trust Functions
    // =========================================================

    public void AddTrust(int amount)
    {
        trust = Mathf.Clamp(
            trust + amount,
            0,
            MaxTrust
        );
    }


    public void AddTrustChanges(int amount)
    {
        if (IsHighRiskHighReturnActive() &&
            amount < 0)
        {
            amount *= 2;
        }

        trustChange += amount;
    }


    public void ResetTrustChanges()
    {
        trustChange = 0;
    }


    public void ApplyTrustChanges()
    {
        AddTrust(trustChange);
        ResetTrustChanges();
    }


    public string GetTrustGrade()
    {
        if (trust >= TrustGradeA)
        {
            return "A";
        }

        if (trust >= TrustGradeB)
        {
            return "B";
        }

        return "C";
    }


    // =========================================================
    // Human Parts Functions
    // =========================================================

    public void ObtainHumanHead()
    {
        if (humanHead)
            return;

        humanHead = true;

        Debug.Log("인간 파츠 획득: 머리");

        RefreshHumanPartUI();
    }


    public void ObtainHumanBody()
    {
        if (humanBody)
            return;

        humanBody = true;

        Debug.Log("인간 파츠 획득: 몸");

        RefreshHumanPartUI();
    }


    public void ObtainHumanHeart()
    {
        if (humanHeart)
            return;

        humanHeart = true;

        Debug.Log("인간 파츠 획득: 심장");

        RefreshHumanPartUI();
    }


    private void RefreshHumanPartUI()
    {
        if (HumanPartInventoryUI.Instance != null)
        {
            HumanPartInventoryUI.Instance.RefreshAll();
        }
    }


    public bool HasAllHumanParts()
    {
        return humanBody &&
               humanHead &&
               humanHeart;
    }


    // =========================================================
    // Skill - Fuel Recovery
    // =========================================================

    public int GetFuelRecoveryAmount()
    {
        switch (fuelRecoveryLevel)
        {
            case 1:
                return fuelRecoveryLv1;

            case 2:
                return fuelRecoveryLv2;

            case 3:
                return fuelRecoveryLv3;

            default:
                return 0;
        }
    }


    // =========================================================
    // Skill - Trust Recovery
    // =========================================================

    public int GetTrustRecoveryAmount()
    {
        switch (trustRecoveryLevel)
        {
            case 1:
                return trustRecoveryLv1;

            case 2:
                return trustRecoveryLv2;

            case 3:
                return trustRecoveryLv3;

            default:
                return 0;
        }
    }


    // =========================================================
    // Skill - High Risk High Return
    // =========================================================

    public bool IsHighRiskHighReturnActive()
    {
        return highRiskHighReturnLevel >= 1;
    }


    // =========================================================
    // Skill - Work Time
    // =========================================================

    private void UpdateDayDuration()
    {
        dayDuration =
            baseDayDuration +
            (workTimeLevel * workTimeBonusPerLevel);
    }


    public int GetWorkEndTimeMinutes()
    {
        int baseEndMinutes =
            15 * 60;

        int bonusMinutes =
            workTimeLevel * 30;

        return baseEndMinutes + bonusMinutes;
    }


    public int GetWorkEndHour()
    {
        return GetWorkEndTimeMinutes() / 60;
    }


    public int GetWorkEndMinute()
    {
        return GetWorkEndTimeMinutes() % 60;
    }


    // =========================================================
    // Day Start
    // =========================================================

    public void StartDay()
    {
        // 연료 자동회복
        int fuelRecovery =
            GetFuelRecoveryAmount();

        if (fuelRecovery > 0)
        {
            AddFuel(fuelRecovery);

            Debug.Log(
                $"스킬 효과 - 연료 +{fuelRecovery}"
            );
        }


        // 신뢰도 자동회복
        int trustRecovery =
            GetTrustRecoveryAmount();

        if (trustRecovery > 0)
        {
            AddTrust(trustRecovery);

            Debug.Log(
                $"스킬 효과 - 신뢰도 +{trustRecovery}"
            );
        }


        // 작업시간 재계산
        UpdateDayDuration();


        Debug.Log(
            $"Day Start / 실제 작업시간 {dayDuration}초 / " +
            $"게임 종료시각 {GetWorkEndHour():00}:{GetWorkEndMinute():00}"
        );
    }


    // =========================================================
    // Day
    // =========================================================

    public void NextDay()
    {
        // 세 파츠를 전부 가진 상태에서
        // 다음 날로 넘어갈 때만 인간 상태 활성화
        if (HasAllHumanParts())
        {
            isHuman = true;

            Debug.Log(
                "모든 인간 파츠를 보유했습니다. 인간 상태가 활성화됩니다."
            );
        }


        mistakeNumber = 0;
        comboNumber = 0;


        if (currentDay > 0)
        {
            currentDay--;

            Debug.Log(
                $"다음 날! 현재 날짜 : D-{currentDay}"
            );
        }
        else
        {
            Debug.Log(
                "D-Day입니다."
            );
        }
    }


    // =========================================================
    // Skill Purchase - Fuel Recovery
    // =========================================================

    public bool PurchaseFuelRecovery(int cost)
    {
        if (fuelRecoveryLevel >= 3)
        {
            Debug.Log(
                "연료 회복 스킬은 최대 레벨입니다."
            );

            return false;
        }


        if (!SpendMoney(cost))
            return false;


        fuelRecoveryLevel++;


        Debug.Log(
            $"연료 자동회복 Lv.{fuelRecoveryLevel} 구매"
        );


        return true;
    }


    // =========================================================
    // Skill Purchase - Trust Recovery
    // =========================================================

    public bool PurchaseTrustRecovery(int cost)
    {
        if (trustRecoveryLevel >= 3)
        {
            Debug.Log(
                "신뢰 회복 스킬은 최대 레벨입니다."
            );

            return false;
        }


        if (!SpendMoney(cost))
            return false;


        trustRecoveryLevel++;


        Debug.Log(
            $"신뢰 자동회복 Lv.{trustRecoveryLevel} 구매"
        );


        return true;
    }


    // =========================================================
    // Skill Purchase - High Risk High Return
    // =========================================================

    public bool PurchaseHighRiskHighReturn(int cost)
    {
        if (highRiskHighReturnLevel >= 1)
        {
            Debug.Log(
                "하이리스크 하이리턴 스킬을 이미 구매했습니다."
            );

            return false;
        }


        if (!SpendMoney(cost))
            return false;


        highRiskHighReturnLevel = 1;


        Debug.Log(
            "하이리스크 하이리턴 구매"
        );


        return true;
    }


    // =========================================================
    // Skill Purchase - Work Time
    // =========================================================

    public bool PurchaseWorkTime(int cost)
    {
        if (workTimeLevel >= 3)
        {
            Debug.Log(
                "작업시간 증가 스킬은 최대 레벨입니다."
            );

            return false;
        }


        if (!SpendMoney(cost))
            return false;


        workTimeLevel++;


        UpdateDayDuration();


        Debug.Log(
            $"작업시간 증가 Lv.{workTimeLevel} 구매 / " +
            $"실제 작업시간 {dayDuration}초 / " +
            $"종료시각 {GetWorkEndHour():00}:{GetWorkEndMinute():00}"
        );


        return true;
    }
}