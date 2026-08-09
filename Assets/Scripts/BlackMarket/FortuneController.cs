using UnityEngine;
using TMPro;

public class FortuneController : MonoBehaviour
{
    [System.Serializable]
    public class FortuneData
    {
        [TextArea(2, 5)]
        public string fortuneText;
    }

    [Header("오늘의 운세 페이지")]
    [SerializeField] private GameObject fortunePanel;

    [Header("운세 표시 텍스트")]
    [SerializeField] private TMP_Text fortuneText;

    [Header("운세 목록")]
    [SerializeField] private FortuneData[] fortunes;


    // ================================
    // 선택된 운세의 돈 변화량
    // ================================
    public int MoneyChange { get; private set; } = 0;


    // ================================
    // 선택된 운세 인덱스
    // ================================
    public int SelectedFortuneIndex { get; private set; } = -1;


    // ================================
    // 버튼에서 호출
    // ================================
    public void OpenFortune()
    {
        if (fortunePanel != null)
        {
            fortunePanel.SetActive(true);
        }

        SelectRandomFortune();
    }


    private void SelectRandomFortune()
    {
        if (fortunes == null || fortunes.Length == 0)
        {
            Debug.LogWarning("운세가 등록되어 있지 않습니다.");
            return;
        }

        // 모든 운세 동일 확률
        SelectedFortuneIndex = Random.Range(0, fortunes.Length);

        FortuneData selectedFortune = fortunes[SelectedFortuneIndex];


        // 운세 문구 표시
        if (fortuneText != null)
        {
            fortuneText.text = selectedFortune.fortuneText;
        }


        // ================================
        // 운세에 따른 실제 돈 처리
        // ================================

        // --------------------------------
        // 첫 번째 운세 (인덱스 0)
        // 가진 돈 2배
        // --------------------------------
        if (SelectedFortuneIndex == 0)
        {
            int currentMoney = PlayerStatus.Instance.money;

            PlayerStatus.Instance.AddMoney(currentMoney);

            Debug.Log(
                $"대박! 가진 돈이 2배가 되었습니다. " +
                $"현재 소지금: {PlayerStatus.Instance.money}"
            );
        }


        // --------------------------------
        // 마지막 운세 (인덱스 9)
        // 가진 돈 전부 몰수
        // --------------------------------
        else if (SelectedFortuneIndex == fortunes.Length - 1)
        {
            int currentMoney = PlayerStatus.Instance.money;

            PlayerStatus.Instance.SpendMoney(currentMoney);

            Debug.Log(
                $"최악의 운세! 가진 돈을 전부 몰수당했습니다. " +
                $"현재 소지금: {PlayerStatus.Instance.money}"
            );
        }

        // 테스트용 로그
        Debug.Log(
            $"오늘의 운세: {selectedFortune.fortuneText} / " +
            $"인덱스: {SelectedFortuneIndex} / " +
            $"돈 변화: {MoneyChange} / " +
            $"현재 소지금: {PlayerStatus.Instance.money}"
        );
    }
}