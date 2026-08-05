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

    // 3. 연료
    [Header("Fuel")]
    [Range(0, 100)]
    public int fuel;

    public const int MaxFuel = 100;

    // 4. 신뢰도
    [Header("Trust")]
    [Range(0, 100)]
    public int trust;

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

    // ---------------------------------------------------------------------

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

    public void ReduceTrust(int amount)
    {
        trust = Mathf.Clamp(trust - amount, 0, MaxTrust);
    }
}