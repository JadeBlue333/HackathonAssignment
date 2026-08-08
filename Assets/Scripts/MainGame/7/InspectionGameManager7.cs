using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InspectionGameManager7 : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private BoxMatchManager boxMatchManager;
    [SerializeField] private MotorMatchManager7 motorMatchManager;

    [Header("Box / Motor")]
    public GameObject boxButtons;
    public GameObject motorButtons;

    [Header("Reward")]
    [SerializeField] private int unopenedReward = 3;
    [SerializeField] private int aReward = 2;
    [SerializeField] private int bReward = 1;
    [SerializeField] private int cReward = 1;
    [SerializeField] private int discardReward = 0;

    [Header("Penalty")]
    [SerializeField] private int trustPenalty = 5;
    [SerializeField] private int fuelCost = 2;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip spawnBoxSfx;
    [Range(0f, 1f)]
    [SerializeField] private float spawnBoxVolume = 1f;
    [SerializeField] private AudioClip openBoxSfx;
    [Range(0f, 1f)]
    [SerializeField] private float openBoxVolume = 1f;
    [SerializeField] private AudioClip correctSfx;
    [Range(0f, 1f)]
    [SerializeField] private float correctVolume = 1f;
    [SerializeField] private AudioClip wrongSfx;
    [Range(0f, 1f)]
    [SerializeField] private float wrongVolume = 1f;

    private int correctCount = 0;
    private int wrongCount = 0;
    private int comboCount = 0;

    //열지 않았어도 될 상자를 연건지.
    private bool openedWrongBox = false;

    private InspectionResult currentAnswer;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => boxMatchManager.IsReady && motorMatchManager.IsReady);

        GenerateQuestion();
    }

    private void Update()
    {
        if (boxButtons.activeSelf)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SelectUnopened();
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SelectOpened();
            }

            return;
        }

        if (motorButtons.activeSelf)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                SelectA();
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                SelectB();
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                SelectC();
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                SelectDiscard();
            }
        }
    }

    private void PlaySfx(
        AudioClip clip,
        float volume
    )
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(
            clip,
            volume
        );
    }

    void GenerateQuestion()
    {
        openedWrongBox = false;

        currentAnswer = boxMatchManager.CreateNextMatch();

        PlaySfx(
            spawnBoxSfx,
            spawnBoxVolume
        );

        StartCoroutine(nextQuestionDelay());

        Debug.Log("==========================");
        Debug.Log($"새 문제 생성");
        Debug.Log($"정답 : {currentAnswer}");
    }

    // 다음 문제 버튼 활성화 딜레이. 피로도 덜하게 . . .
    private IEnumerator nextQuestionDelay()
    {
        yield return new WaitForSeconds(0.5f);
        boxButtons.SetActive(true);
    }

    public void SelectUnopened()
    {
        CheckAnswer(InspectionResult.Unopened);
        boxButtons.SetActive(false);
    }

    public void SelectOpened()
    {
        PlaySfx(
            openBoxSfx,
            openBoxVolume
        );

        boxButtons.SetActive(false);

        openedWrongBox = (currentAnswer == InspectionResult.Unopened);

        boxMatchManager.RemoveCurrentMatch();
        currentAnswer = motorMatchManager.CreateNextMatch();

        motorButtons.SetActive(true);

        Debug.Log($"모터 정답 : {currentAnswer}");
    }

    public void SelectA()
    {
        motorButtons.SetActive(false);
        CheckAnswer(InspectionResult.A);

        motorMatchManager.RemoveCurrentMotor();
    }

    public void SelectB()
    {
        motorButtons.SetActive(false);
        CheckAnswer(InspectionResult.B);

        motorMatchManager.RemoveCurrentMotor();
    }

    public void SelectC()
    {
        motorButtons.SetActive(false);
        CheckAnswer(InspectionResult.C);

        motorMatchManager.RemoveCurrentMotor();
    }

    public void SelectDiscard()
    {
        motorButtons.SetActive(false);
        CheckAnswer(InspectionResult.Discard);

        motorMatchManager.RemoveCurrentMotor();
    }

    void CheckAnswer(InspectionResult playerAnswer)
    {
        bool correct;

        if (openedWrongBox)
        {
            // 안 열어도 되는 박스를 열었으면 모터를 아무리 맞혀도 실패
            correct = false;
        }
        else
        {
            correct = playerAnswer == currentAnswer;
        }

        Debug.Log($"선택 : {playerAnswer}");
        Debug.Log($"정답 : {currentAnswer}");

        if (correct)
        {
            PlaySfx(
                correctSfx,
                correctVolume
            );

            // 정답 개수 증가
            correctCount++;

            // 콤보 증가
            comboCount++;

            if (comboCount == 3)
            {
                PlayerStatus.Instance
                    .AddTrustChanges(2);

                Debug.Log(
                    "★★★ 3 COMBO! 신뢰도 +2 ★★★"
                );

                PlayerStatus.Instance.comboNumber++;

                comboCount = 0;
            }

            int reward = GetReward(currentAnswer);

            PlayerStatus.Instance.AddEarnings(reward);

            Debug.Log($"정답! +{reward} 크레타");
        }
        else
        {
            PlaySfx(
                wrongSfx,
                wrongVolume
            );

            wrongCount++;
            PlayerStatus.Instance.mistakeNumber++;

            comboCount = 0;

            PlayerStatus.Instance.AddTrustChanges(-trustPenalty);

            Debug.Log($"오답! 신뢰도 -{trustPenalty}");
        }

        PlayerStatus.Instance.AddFuel(-fuelCost);

        Debug.Log($"연료 -{fuelCost}");

        if (PlayerStatus.Instance.trust <= 0)
        {
            Debug.Log("GAME OVER - 신뢰도 부족");
            return;
        }

        if (PlayerStatus.Instance.fuel <= 0)
        {
            Debug.Log("GAME OVER - 연료 부족");
            return;
        }

        GenerateQuestion();
    }

    int GetReward(InspectionResult result)
    {
        switch (result)
        {
            case InspectionResult.Unopened:
                return unopenedReward;

            case InspectionResult.A:
                return aReward;

            case InspectionResult.B:
                return bReward;

            case InspectionResult.C:
                return cReward;

            case InspectionResult.Discard:
                return discardReward;
        }

        return 0;
    }
}