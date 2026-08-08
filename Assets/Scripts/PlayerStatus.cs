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

    private void Initialize()
    {
        // 초기 상태
        currentDay = 9;     // D-9
        money = 20;

        fuel = 70;
        trust = 50;

        earnings = 0;
        trustChange = 0;

        comboNumber = 0;
        mistakeNumber = 0;
    }

    [Header("Inspector에서 수정해도 게임 시작 시 Initialize 값으로 초기화됩니다.")]

    // =========================================================
    // Date
    // =========================================================

    [Header("Date")]
    [Range(0, 9)]
    public int currentDay;      // 9 -> 0 (D-Day)


    // =========================================================
    // Money
    // =========================================================

    [Header("Money")]
    public int money;

    // 이번 하루 동안 벌어들인 금액
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

    // 이번 하루 동안 발생한 신뢰도 변화량
    public int trustChange;

    public const int MaxTrust = 100;

    // 신뢰도 등급 기준
    public const int TrustGradeA = 70;
    public const int TrustGradeB = 40;


    // =========================================================
    // Enhancement Items
    // =========================================================

    [Header("Enhancement Items")]
    public bool[] enhancementItems = new bool[3];


    // =========================================================
    // Skill Tree
    // =========================================================

    [Header("Skill Tree")]
    public bool[] upgrades = new bool[4];


    // =========================================================
    // Human Parts
    // =========================================================

    [Header("Human Parts")]
    public bool[] humanParts = new bool[3];


    // =========================================================
    // Time
    // =========================================================

    [Header("Time")]
    [Tooltip("하루 실제 플레이 시간(초)")]
    public float dayDuration = 180f;


    [Header("콤보 / 실수")]
    public int comboNumber = 0;
    public int mistakeNumber = 0;

    // =========================================================
    // Money Functions
    // =========================================================

    // 이번 하루 수익 추가
    public void AddEarnings(int amount)
    {
        earnings += amount;
    }

    // 이번 하루 수익 초기화
    public void ResetEarnings()
    {
        earnings = 0;
    }

    // 하루 종료 시 수익을 실제 보유 금액에 반영
    public void ApplyEarnings()
    {
        AddMoney(earnings);
        ResetEarnings();
    }

    // 돈 추가
    public void AddMoney(int amount)
    {
        money += amount;
    }

    // 돈 사용
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

    // 연료 추가
    public void AddFuel(int amount)
    {
        fuel = Mathf.Clamp(
            fuel + amount,
            0,
            MaxFuel
        );
    }

    // 연료 감소
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

    // 신뢰도 즉시 변경
    public void AddTrust(int amount)
    {
        trust = Mathf.Clamp(
            trust + amount,
            0,
            MaxTrust
        );
    }

    // 이번 하루 신뢰도 변화량 누적
    public void AddTrustChanges(int amount)
    {
        trustChange += amount;
    }

    // 신뢰도 변화량 초기화
    public void ResetTrustChanges()
    {
        trustChange = 0;
    }

    // 하루 종료 시 신뢰도 변화 적용
    public void ApplyTrustChanges()
    {
        AddTrust(trustChange);
        ResetTrustChanges();
    }

    // 현재 신뢰도 등급 반환
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
    // Day
    // =========================================================

    public void NextDay()
    {
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
            Debug.Log("D-Day입니다.");
        }
    }


}