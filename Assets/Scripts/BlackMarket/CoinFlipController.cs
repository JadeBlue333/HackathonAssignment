using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CoinFlipController : MonoBehaviour
{
    [Header("코인 플립 페이지")]
    [SerializeField] private GameObject coinFlipPanel;


    [Header("결과 표시 텍스트")]
    [SerializeField] private TMP_Text resultText;


    [Header("최종 소지금 UI")]
    [SerializeField] private GameObject totalMoneyObject;
    [SerializeField] private TMP_Text totalMoneyText;


    [Header("연출 설정")]
    [SerializeField] private float flipDuration = 1.5f;
    [SerializeField] private float changeInterval = 0.1f;


    [Header("최종 결과 확률 (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float frontChance = 50f;


    [Header("코인 비용 / 보상")]
    [SerializeField] private int flipCost = 10;
    [SerializeField] private int successReward = 20;


    [Header("사운드")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip coinFlipSfx;

    [Range(0f, 1f)]
    [SerializeField] private float coinFlipVolume = 1f;


    [SerializeField] private AudioClip successSfx;

    [Range(0f, 1f)]
    [SerializeField] private float successVolume = 1f;


    [SerializeField] private AudioClip failSfx;

    [Range(0f, 1f)]
    [SerializeField] private float failVolume = 1f;


    private bool isFlipping = false;

    private CoinSide playerChoice;


    // ================================
    // 결과값
    //
    // 플레이어가 맞추면 true
    // 플레이어가 틀리면 false
    // ================================
    public bool IsCorrect { get; private set; } = false;


    private enum CoinSide
    {
        Front,
        Back
    }


    private void Start()
    {
        // 게임 시작 시 총 소지금 UI는 숨김
        if (totalMoneyObject != null)
        {
            totalMoneyObject.SetActive(false);
        }
    }


    // ================================
    // "앞" 버튼에서 호출
    // ================================
    public void ChooseFront()
    {
        if (isFlipping)
            return;

        if (!CanAffordFlip())
            return;

        playerChoice = CoinSide.Front;

        OpenAndStartFlip();
    }


    // ================================
    // "뒤" 버튼에서 호출
    // ================================
    public void ChooseBack()
    {
        if (isFlipping)
            return;

        if (!CanAffordFlip())
            return;

        playerChoice = CoinSide.Back;

        OpenAndStartFlip();
    }


    // ================================
    // 코인 플립 비용을 낼 수 있는지 확인
    // ================================
    private bool CanAffordFlip()
    {
        if (PlayerStatus.Instance == null)
            return false;

        if (PlayerStatus.Instance.money < flipCost)
        {
            Debug.Log(
                "Coin Flip 실패 : 소지금이 부족합니다."
            );

            return false;
        }

        return true;
    }


    // ================================
    // 코인 플립 시작
    // ================================
    private void OpenAndStartFlip()
    {
        // 결과가 나오기 전까지
        // 최종 소지금 UI 숨기기
        if (totalMoneyObject != null)
        {
            totalMoneyObject.SetActive(false);
        }


        // 코인 플립 패널 활성화
        if (coinFlipPanel != null)
        {
            coinFlipPanel.SetActive(true);
        }


        // 코인 돌아가는 소리
        if (
            audioSource != null &&
            coinFlipSfx != null
        )
        {
            audioSource.PlayOneShot(
                coinFlipSfx,
                coinFlipVolume
            );
        }


        StartCoroutine(
            FlipRoutine()
        );
    }


    private IEnumerator FlipRoutine()
    {
        isFlipping = true;


        // ================================
        // 이전 결과 초기화
        // ================================
        IsCorrect = false;


        // ================================
        // 현재 씬이 영어 씬인지 확인
        // 씬 이름에 EN 포함 시 영어
        // ================================
        bool isEnglishScene =
            SceneManager
                .GetActiveScene()
                .name
                .Contains("EN");


        float startTime =
            Time.time;

        bool showFront =
            true;


        // ================================
        // 앞 / 뒤가 빠르게 번갈아 보이는 연출
        // 영어 씬에서는 Front / Back
        // ================================
        while (
            Time.time - startTime <
            flipDuration
        )
        {
            if (showFront)
            {
                resultText.text =
                    isEnglishScene
                        ? "Front"
                        : "앞";
            }
            else
            {
                resultText.text =
                    isEnglishScene
                        ? "Back"
                        : "뒤";
            }


            showFront =
                !showFront;


            yield return new WaitForSeconds(
                changeInterval
            );
        }


        // ================================
        // 최종 코인 결과 결정
        // ================================
        CoinSide finalResult;


        if (
            Random.Range(
                0f,
                100f
            ) < frontChance
        )
        {
            finalResult =
                CoinSide.Front;
        }
        else
        {
            finalResult =
                CoinSide.Back;
        }


        // ================================
        // 최종 결과 화면에 표시
        // ================================
        if (
            finalResult ==
            CoinSide.Front
        )
        {
            resultText.text =
                isEnglishScene
                    ? "Front"
                    : "앞";
        }
        else
        {
            resultText.text =
                isEnglishScene
                    ? "Back"
                    : "뒤";
        }


        // ================================
        // 플레이어 선택과 실제 결과 비교
        // ================================
        IsCorrect =
            playerChoice ==
            finalResult;


        // ================================
        // 성공 / 실패 처리
        // ================================
        if (IsCorrect)
        {
            PlayerStatus.Instance
                .AddMoney(
                    successReward
                );


            if (
                audioSource != null &&
                successSfx != null
            )
            {
                audioSource.PlayOneShot(
                    successSfx,
                    successVolume
                );
            }


            Debug.Log(
                "Coin Flip 성공 / Result = True / +" +
                successReward +
                "C"
            );
        }
        else
        {
            if (
                audioSource != null &&
                failSfx != null
            )
            {
                audioSource.PlayOneShot(
                    failSfx,
                    failVolume
                );
            }


            Debug.Log(
                "Coin Flip 실패 / Result = False"
            );
        }


        // ================================
        // 최종 소지금 표시
        // 영어 씬에서는 Total Creta
        // ================================
        if (totalMoneyText != null)
        {
            if (isEnglishScene)
            {
                totalMoneyText.text =
                    "Total Creta : " +
                    PlayerStatus.Instance.money +
                    " C";
            }
            else
            {
                totalMoneyText.text =
                    "총 소지금 : " +
                    PlayerStatus.Instance.money +
                    " C";
            }
        }


        if (totalMoneyObject != null)
        {
            totalMoneyObject.SetActive(
                true
            );
        }


        isFlipping =
            false;
    }
}