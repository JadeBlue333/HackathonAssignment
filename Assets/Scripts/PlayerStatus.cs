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
        currentDay = 9;     // D-9 시작
        money = 20;          
        fuel = 70;
        trust = 50;
    }

    [Header("인스펙터에서 변수 조정하는 것은 의미 X. 시작 변수 고치려면 코드의 Initialize()에서 변경할 것.")]

    // 1. 날짜
    [Header("Date")]
    [Range(0, 9)]
    public int currentDay;      // 9 -> 0 (D-Day)

    // 2. 돈
    [Header("Money")]
    public int money;
    public int earnings; // 이번 날 정산으로 얻은 돈

    // 3. 연료
    [Header("Fuel")]
    [Range(0, 100)]
    public int fuel;

    public const int MaxFuel = 100;

    // 4. 신뢰도
    [Header("Trust")]
    [Range(0, 100)]
    public int trust;
    public int trustChange; // 이번 날 정산으로 얻은 신뢰도 변화

    public const int MaxTrust = 100;

    // 5. 강화 아이템 구매 여부
    [Header("Enhancement Items")]
    public bool[] enhancementItems = new bool[3];

    // 6. 업그레이드 단계 (스킬트리)
    [Header("Skill Tree")]
    public bool[] upgrades = new bool[4];

    // 7. 인간화 부품 구매 여부
    [Header("Human Parts")]
    public bool[] humanParts = new bool[3];

    [Header("Time")]
    [Tooltip("현실 시간 기준 하루 길이(초)")]
    public float dayDuration = 180f;

    // ---------------------------------------------------------------------

    // 이번 날 수익 누적
    public void AddEarnings(int amount)
    {
        earnings += amount;
    }

    // 이번 날 수익 초기화
    public void ResetEarnings()
    {
        earnings = 0;
    }

    // 정산 완료 이 함수 하나만 호출하면 됨
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
            return false;

        money -= amount;
        return true;
    }

    public void AddFuel(int amount)
    {
        fuel = Mathf.Clamp(fuel + amount, 0, MaxFuel);
    }

    public void ReduceFuel(int amount)
    {
        fuel = Mathf.Clamp(fuel - amount, 0, MaxFuel);
    }

    public void AddTrust(int amount)
    {
        trust = Mathf.Clamp(trust + amount, 0, MaxTrust);
    }

    public void AddTrustChanges(int amount)
    {
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

    public void NextDay()
    {
        if (currentDay > 0)
        {
            currentDay--;
            Debug.Log($"다음 날! 현재 날짜 : D-{currentDay}");
        }
        else
        {
            Debug.Log("D-Day입니다.");
        }
    }
}