using UnityEngine;
using TMPro;

public class FortuneController : MonoBehaviour
{
    [System.Serializable]
    public class FortuneData
    {
        [TextArea(2, 5)]
        public string fortuneText;

        [Tooltip("돈 증감값. 예: 100 = +100 / -50 = -50")]
        public int moneyChange;
    }

    [Header("오늘의 운세 페이지")]
    [SerializeField] private GameObject fortunePanel;

    [Header("운세 표시 텍스트")]
    [SerializeField] private TMP_Text fortuneText;

    [Header("운세 목록")]
    [SerializeField] private FortuneData[] fortunes;


    // ================================
    // 나중에 다른 시스템으로 넘길 데이터
    // 선택된 운세의 돈 증감값
    //
    // 예:
    // +100 → 돈 100 증가
    // -50  → 돈 50 감소
    //
    // 다른 스크립트에서
    // fortuneController.MoneyChange
    // 로 가져가면 됨.
    // ================================
    public int MoneyChange { get; private set; } = 0;


    // ================================
    // 어떤 운세가 선택됐는지 저장
    // 0부터 시작
    // ================================
    public int SelectedFortuneIndex { get; private set; } = -1;


    // 버튼에서 이 함수 호출
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

        // 운세 중 하나 랜덤 선택
        SelectedFortuneIndex = Random.Range(0, fortunes.Length);

        FortuneData selectedFortune =
            fortunes[SelectedFortuneIndex];


        // 화면에 운세 문구 표시
        if (fortuneText != null)
        {
            fortuneText.text = selectedFortune.fortuneText;
        }


        // ================================
        // [데이터 전달용 결과 저장 부분]
        //
        // 선택된 운세에 설정된 돈 증감값 저장
        //
        // 나중에 실제 플레이어 돈 시스템에
        // 이 값을 전달하면 됨.
        // ================================
        MoneyChange = selectedFortune.moneyChange;


        // 테스트용
        Debug.Log(
            $"오늘의 운세: {selectedFortune.fortuneText} / " +
            $"돈 변화: {MoneyChange}"
        );
    }
}