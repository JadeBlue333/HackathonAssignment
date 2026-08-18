using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private int slotNumber;

    [Header("Texts")]
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private TMP_Text emptyText;

    [Header("Endings")]
    public Image ending1;
    public Image ending2;
    public Image ending3;
    public Image ending4;
    public Image ending5;

    [Header("Scene Transition")]
    [SerializeField] private GoToThisScene goToThisScene;


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

        if (!LanguageManager.Instance.isEnglish)
        {
            infoText.text =
                $"D - {data.currentDay}\n" +
                $"연료: {data.fuel}\n" +
                $"신뢰도: {data.trust}\n" +
                $"크레타: {data.money}\n\n" +
                $"{data.saveDate}";
        }
        else
        {
            infoText.text =
                $"D - {data.currentDay}\n" +
                $"Fuel: {data.fuel}\n" +
                $"Trust: {data.trust}\n" +
                $"Creta: {data.money}\n\n" +
                $"{data.saveDate}";
        }

        SetEndingAlpha(ending1, data.ending1Achieved);
        SetEndingAlpha(ending2, data.ending2Achieved);
        SetEndingAlpha(ending3, data.ending3Achieved);
        SetEndingAlpha(ending4, data.ending4Achieved);
        SetEndingAlpha(ending5, data.ending5Achieved);
    }

    private void SetEndingAlpha(Image image, bool achieved)
    {
        Color color = image.color;
        color.a = achieved ? 1f : 0.2f;
        image.color = color;
    }

    public void Save()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다.");
            return;
        }

        bool success = SaveManager.Instance.SaveGame(slotNumber);

        if (success)
        {
            Refresh();
        }
    }


    public void Load()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 없습니다.");
            return;
        }

        bool success = SaveManager.Instance.LoadGame(slotNumber);

        // 로드 실패하면 여기서 끝
        if (!success)
            return;

        // 로드 성공했을 때만 씬 이동
        if (goToThisScene == null)
        {
            Debug.LogError("GoToThisScene이 연결되지 않았습니다.");
            return;
        }

        goToThisScene.nextSceneButton();
    }
}