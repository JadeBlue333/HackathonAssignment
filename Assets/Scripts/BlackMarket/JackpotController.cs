using System.Collections;
using UnityEngine;
using TMPro;

public class JackpotController : MonoBehaviour
{
    [Header("잭팟 페이지")]
    [SerializeField] private GameObject jackpotPanel;

    [Header("숫자 UI")]
    [SerializeField] private TMP_Text numberText1;
    [SerializeField] private TMP_Text numberText2;
    [SerializeField] private TMP_Text numberText3;

    [Header("7 등장 확률 (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float sevenChance = 20f;

    [Header("숫자가 바뀌는 속도")]
    [SerializeField] private float numberChangeInterval = 0.05f;

    [Header("각 숫자가 멈추는 시간")]
    [SerializeField] private float firstStopTime = 0.5f;
    [SerializeField] private float secondStopTime = 0.8f;
    [SerializeField] private float thirdStopTime = 1.1f;

    private bool isSpinning = false;

    // ================================
    // 나중에 다른 시스템으로 넘길 잭팟 결과값
    // 777이면 true
    // 그 외 모든 결과는 false
    // ================================
    public bool IsJackpot { get; private set; } = false;


    public void OpenJackpotAndSpin()
    {
        if (isSpinning)
            return;

        if (jackpotPanel != null)
        {
            jackpotPanel.SetActive(true);
        }

        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // 새로운 게임 시작 시 이전 결과 초기화
        IsJackpot = false;

        // 최종 결과 미리 결정
        int result1 = GetRandomNumber();
        int result2 = GetRandomNumber();
        int result3 = GetRandomNumber();

        float startTime = Time.time;

        bool firstStopped = false;
        bool secondStopped = false;
        bool thirdStopped = false;

        while (!thirdStopped)
        {
            float elapsedTime = Time.time - startTime;

            // 첫 번째 숫자
            if (!firstStopped)
            {
                if (elapsedTime >= firstStopTime)
                {
                    numberText1.text = result1.ToString();
                    firstStopped = true;
                }
                else
                {
                    numberText1.text = Random.Range(0, 10).ToString();
                }
            }

            // 두 번째 숫자
            if (!secondStopped)
            {
                if (elapsedTime >= secondStopTime)
                {
                    numberText2.text = result2.ToString();
                    secondStopped = true;
                }
                else
                {
                    numberText2.text = Random.Range(0, 10).ToString();
                }
            }

            // 세 번째 숫자
            if (!thirdStopped)
            {
                if (elapsedTime >= thirdStopTime)
                {
                    numberText3.text = result3.ToString();
                    thirdStopped = true;
                }
                else
                {
                    numberText3.text = Random.Range(0, 10).ToString();
                }
            }

            yield return new WaitForSeconds(numberChangeInterval);
        }

        // ================================
        // [데이터 전달용 결과 저장 부분]
        //
        // 3개의 결과가 모두 7이면 true
        // 하나라도 7이 아니면 false
        //
        // 나중에 다른 스크립트에서
        // jackpotController.IsJackpot
        // 으로 이 값을 가져가면 됨.
        // ================================
        IsJackpot =
            result1 == 7 &&
            result2 == 7 &&
            result3 == 7;


        // 테스트용 콘솔 출력
        if (IsJackpot)
        {
            Debug.Log("JACKPOT! 777 / Result = True");
        }
        else
        {
            Debug.Log($"결과: {result1}{result2}{result3} / Result = False");
        }

        isSpinning = false;
    }

    private int GetRandomNumber()
    {
        if (Random.Range(0f, 100f) < sevenChance)
        {
            return 7;
        }

        int number;

        do
        {
            number = Random.Range(0, 10);
        }
        while (number == 7);

        return number;
    }
}