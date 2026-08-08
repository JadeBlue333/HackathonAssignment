using System.Collections;
using UnityEngine;
using TMPro;

public class CoinFlipController : MonoBehaviour
{
    [Header("코인 플립 페이지")]
    [SerializeField] private GameObject coinFlipPanel;

    [Header("결과 표시 텍스트")]
    [SerializeField] private TMP_Text resultText;

    [Header("연출 설정")]
    [SerializeField] private float flipDuration = 1.5f;
    [SerializeField] private float changeInterval = 0.1f;

    [Header("최종 결과 확률 (%)")]
    [Range(0f, 100f)]
    [SerializeField] private float frontChance = 50f;

    private bool isFlipping = false;

    private CoinSide playerChoice;


    // ================================
    // 나중에 다른 시스템으로 넘길 결과값
    //
    // 플레이어가 맞추면 true
    // 플레이어가 틀리면 false
    //
    // 다른 스크립트에서는
    // coinFlipController.IsCorrect
    // 로 가져가면 됨.
    // ================================
    public bool IsCorrect { get; private set; } = false;


    private enum CoinSide
    {
        Front,
        Back
    }


    // ================================
    // "앞" 버튼에서 호출
    // ================================
    public void ChooseFront()
    {
        if (isFlipping)
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

        playerChoice = CoinSide.Back;

        OpenAndStartFlip();
    }


    private void OpenAndStartFlip()
    {
        if (coinFlipPanel != null)
        {
            coinFlipPanel.SetActive(true);
        }

        StartCoroutine(FlipRoutine());
    }


    private IEnumerator FlipRoutine()
    {
        isFlipping = true;

        // 이전 결과 초기화
        IsCorrect = false;

        float startTime = Time.time;

        bool showFront = true;

        // ================================
        // 앞 / 뒤가 빠르게 번갈아 보이는 연출
        // ================================
        while (Time.time - startTime < flipDuration)
        {
            if (showFront)
            {
                resultText.text = "앞";
            }
            else
            {
                resultText.text = "뒤";
            }

            showFront = !showFront;

            yield return new WaitForSeconds(changeInterval);
        }


        // ================================
        // 최종 코인 결과 결정
        //
        // Front Chance 값에 따라
        // 앞면이 나올 확률을 Inspector에서 조절 가능
        // ================================
        CoinSide finalResult;

        if (Random.Range(0f, 100f) < frontChance)
        {
            finalResult = CoinSide.Front;
        }
        else
        {
            finalResult = CoinSide.Back;
        }


        // 최종 결과 화면에 표시
        if (finalResult == CoinSide.Front)
        {
            resultText.text = "앞";
        }
        else
        {
            resultText.text = "뒤";
        }


        // ================================
        // [데이터 전달용 결과 저장 부분]
        //
        // 플레이어가 선택한 앞/뒤와
        // 실제 결과가 같으면 true
        //
        // 다르면 false
        //
        // 나중에 다른 시스템에서
        // coinFlipController.IsCorrect
        // 로 가져가면 됨.
        // ================================
        IsCorrect = playerChoice == finalResult;


        // 테스트용 콘솔 출력
        if (IsCorrect)
        {
            Debug.Log("Coin Flip 성공 / Result = True");
        }
        else
        {
            Debug.Log("Coin Flip 실패 / Result = False");
        }

        isFlipping = false;
    }
}