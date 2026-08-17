using TMPro;
using UnityEngine;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private int slotNumber;

    [Header("Texts")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text emptyText;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다.");
            return;
        }

        PlayerStatus.ProgressSnapshot data =
            SaveManager.Instance.GetSaveData(slotNumber);

        // 저장 데이터가 없는 경우
        if (data == null)
        {
            infoText.gameObject.SetActive(false);

            emptyText.gameObject.SetActive(true);

            return;
        }

        // 저장 데이터가 있는 경우
        infoText.gameObject.SetActive(true);

        emptyText.gameObject.SetActive(false);

        infoText.text = $"D - {data.currentDay}\n연료: {data.fuel}\n신뢰도: {data.trust}\n크레타: {data.money}\n\n{data.saveDate}";
    }

    public void Save()
    {
        SaveManager.Instance.SaveGame(slotNumber);

        Refresh();
    }

    public void Load()
    {
        SaveManager.Instance.LoadGame(slotNumber);
    }
}