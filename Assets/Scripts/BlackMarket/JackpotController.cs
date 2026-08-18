using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class JackpotController : MonoBehaviour
{
    [Header("잭팟 페이지")]
    [SerializeField] private GameObject jackpotPanel;

    [Header("숫자 UI")]
    [SerializeField] private TMP_Text numberText1;
    [SerializeField] private TMP_Text numberText2;
    [SerializeField] private TMP_Text numberText3;

    [Header("최종 소지금 UI")]
    [SerializeField] private GameObject totalMoneyObject;
    [SerializeField] private TMP_Text totalMoneyText;

    [SerializeField] private int inputMoney = 0;

    [Header("7 등장 확률 (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float sevenChance = 20f;

    [Header("숫자가 바뀌는 속도")]
    [SerializeField] private float numberChangeInterval = 0.05f;

    [Header("각 숫자가 멈추는 시간")]
    [SerializeField] private float firstStopTime = 0.5f;
    [SerializeField] private float secondStopTime = 0.8f;
    [SerializeField] private float thirdStopTime = 1.1f;


    // =====================================================
    // Sound
    // =====================================================

    [Header("사운드")]
    [SerializeField] private AudioSource audioSource;

    [Header("룰렛 회전 사운드")]
    [SerializeField] private AudioClip spinSound;

    [Range(0f, 1f)]
    [SerializeField] private float spinVolume = 1f;

    [Header("잭팟 성공 사운드")]
    [SerializeField] private AudioClip jackpotSound;

    [Range(0f, 1f)]
    [SerializeField] private float jackpotVolume = 1f;

    [Header("잭팟 실패 사운드")]
    [SerializeField] private AudioClip failSound;

    [Range(0f, 1f)]
    [SerializeField] private float failVolume = 1f;


    private bool isSpinning = false;


    // ================================
    // 777이면 true
    // 그 외 모든 결과는 false
    // ================================
    public bool IsJackpot { get; private set; } = false;


    public void OpenJackpotAndSpin()
    {
        if (isSpinning)
            return;

        inputMoney = PlayerStatus.Instance.money;

        // 시작하는 순간 돈 다 써버림
        PlayerStatus.Instance.money = 0;

        Debug.Log(
            $"잭팟 시작! 현재 소지금: {PlayerStatus.Instance.money} / " +
            $"잭팟에 사용한 금액: {inputMoney}"
        );

        // 결과 소지금 UI 숨기기
        if (totalMoneyObject != null)
        {
            totalMoneyObject.SetActive(false);
        }

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


        // =====================================================
        // 룰렛 회전 사운드 시작
        // =====================================================

        if (audioSource != null && spinSound != null)
        {
            audioSource.PlayOneShot(spinSound, spinVolume);
        }


        // =====================================================
        // 최종 결과 미리 결정
        // 앞의 두 숫자는 무조건 7
        // 마지막 숫자만 확률 적용
        // =====================================================

        int result1 = 7;
        int result2 = 7;
        int result3 = GetRandomNumber();


        float startTime = Time.time;

        bool firstStopped = false;
        bool secondStopped = false;
        bool thirdStopped = false;


        // =====================================================
        // 룰렛 돌리기
        // =====================================================

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


        // =====================================================
        // 잭팟 결과 판정
        // =====================================================

        IsJackpot =
            result1 == 7 &&
            result2 == 7 &&
            result3 == 7;


        // =====================================================
        // 성공 / 실패 처리
        // =====================================================

        if (IsJackpot)
        {
            int result = inputMoney * 7;

            PlayerStatus.Instance.AddMoney(result);

            Debug.Log(
                $"JACKPOT! 777 / Result = True / 획득 금액: {result}C"
            );


            // 성공 사운드
            if (audioSource != null && jackpotSound != null)
            {
                audioSource.PlayOneShot(jackpotSound, jackpotVolume);
            }
        }
        else
        {
            Debug.Log(
                $"결과: {result1}{result2}{result3} / Result = False"
            );


            // 실패 사운드
            if (audioSource != null && failSound != null)
            {
                audioSource.PlayOneShot(failSound, failVolume);
            }
        }


        // =====================================================
        // 최종 소지금 표시
        // =====================================================

        if (totalMoneyText != null)
        {
            string sceneName =
                SceneManager.GetActiveScene().name;

            if (sceneName.Contains("EN"))
            {
                totalMoneyText.text =
                    $"Total Creta : {PlayerStatus.Instance.money} C";
            }
            else
            {
                totalMoneyText.text =
                    $"총 소지금 : {PlayerStatus.Instance.money} C";
            }
        }

        if (totalMoneyObject != null)
        {
            totalMoneyObject.SetActive(true);
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