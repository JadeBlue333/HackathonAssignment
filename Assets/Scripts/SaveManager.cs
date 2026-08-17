using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    //1~5 슬롯까지 플레이어가 저장 가능. 6번 슬롯은 플레이어가 조종할 수 없다! 게임 오버용 체크포인트...
    private const int SlotCount = 6;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    //에디터에서 플레이 모드 종료 시 PlayerPrefs 초기화
    //유지하고 싶으면 이 부분 주석처리!
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
#endif
    }

    // =========================================================
    // Save
    // =========================================================

    public void SaveGame(int slot)
    {
        // 슬롯 번호 확인
        if (slot < 1 || slot > SlotCount)
        {
            Debug.LogError($"잘못된 슬롯 번호입니다: {slot}");
            return;
        }

        // PlayerStatus 확인
        if (PlayerStatus.Instance == null)
        {
            Debug.LogError("PlayerStatus.Instance가 없습니다.");
            return;
        }

        // 현재 PlayerStatus 상태를 Snapshot에 복사
        PlayerStatus.Instance.SaveProgressSnapshot();

        // Snapshot 가져오기
        PlayerStatus.ProgressSnapshot data =
            PlayerStatus.Instance.GetProgressSnapshot();

        // JSON으로 변환
        string json = JsonUtility.ToJson(data);

        // 슬롯 번호에 따라 다른 Key 사용
        string key = $"SaveSlot_{slot}";

        // PlayerPrefs에 저장
        PlayerPrefs.SetString(key, json);

        // 실제 저장
        PlayerPrefs.Save();

        Debug.Log($"Slot {slot} 저장 완료");
    }

    // =========================================================
    // Load
    // =========================================================

    public void LoadGameFromButton(int slot)
    {
        LoadGame(slot);
    }

    public bool LoadGame(int slot)
    {
        // 슬롯 번호 확인
        if (slot < 1 || slot > SlotCount)
        {
            Debug.LogError($"잘못된 슬롯 번호입니다: {slot}");
            return false;
        }

        // PlayerStatus 확인
        if (PlayerStatus.Instance == null)
        {
            Debug.LogError("PlayerStatus.Instance가 없습니다.");
            return false;
        }

        // 슬롯에 저장된 데이터가 있는지 확인
        string key = $"SaveSlot_{slot}";

        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning($"Slot {slot}에 저장된 데이터가 없습니다.");
            return false;
        }

        // JSON 가져오기
        string json = PlayerPrefs.GetString(key);

        // JSON → ProgressSnapshot으로 변환
        PlayerStatus.ProgressSnapshot data =
            JsonUtility.FromJson<PlayerStatus.ProgressSnapshot>(json);

        if (data == null)
        {
            Debug.LogError($"Slot {slot} 데이터를 불러오지 못했습니다.");
            return false;
        }

        // Snapshot을 PlayerStatus에 넣기
        PlayerStatus.Instance.SetProgressSnapshot(data);

        // Snapshot의 내용을 실제 PlayerStatus에 적용
        PlayerStatus.Instance.LoadProgressSnapshot();

        Debug.Log($"Slot {slot} 불러오기 완료");

        return true;
    }

    public PlayerStatus.ProgressSnapshot GetSaveData(int slot)
    {
        if (slot < 1 || slot > SlotCount)
        {
            Debug.LogError($"잘못된 슬롯 번호입니다: {slot}");
            return null;
        }

        string key = $"SaveSlot_{slot}";

        if (!PlayerPrefs.HasKey(key))
        {
            return null;
        }

        string json = PlayerPrefs.GetString(key);

        PlayerStatus.ProgressSnapshot data =
            JsonUtility.FromJson<PlayerStatus.ProgressSnapshot>(json);

        return data;
    }

    //게임오버 시 자동으로 저장되는 체크포인트 슬롯(6번)용 함수 -------------------------
    public void SaveDailyCheckpoint()
    {
        SaveGame(6);
    }

    public bool LoadDailyCheckpoint()
    {
        return LoadGame(6);
    }
}