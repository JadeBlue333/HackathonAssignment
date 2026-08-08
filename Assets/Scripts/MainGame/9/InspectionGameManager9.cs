using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class InspectionGameManager9 : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private BoxMatchManager boxMatchManager;
    [SerializeField] private MotorMatchManager9 motorMatchManager;

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

    [Header("Spawn Box SFX")]
    [SerializeField] private AudioClip spawnBoxSfx;

    [Range(0f, 1f)]
    [SerializeField] private float spawnBoxVolume = 1f;

    [Header("Open Box SFX")]
    [SerializeField] private AudioClip openBoxSfx;

    [Range(0f, 1f)]
    [SerializeField] private float openBoxVolume = 1f;

    [Header("Correct SFX")]
    [SerializeField] private AudioClip correctSfx;

    [Range(0f, 1f)]
    [SerializeField] private float correctVolume = 1f;

    [Header("Wrong SFX")]
    [SerializeField] private AudioClip wrongSfx;

    [Range(0f, 1f)]
    [SerializeField] private float wrongVolume = 1f;


    // 정상 박스를 열어버렸는지 여부
    private bool openedWrongBox = false;

    private InspectionResult currentAnswer;

    // =========================================================
    // Combo / Statistics
    // =========================================================

    private int correctCount = 0;
    private int wrongCount = 0;
    private int comboCount = 0;

    private IEnumerator Start()
    {
        yield return new WaitUntil(
            () => boxMatchManager.IsReady &&
                  motorMatchManager.IsReady
        );

        GenerateQuestion();
    }

    private void Update()
    {
        // Box 선택 단계
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


        // Motor 선택 단계
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


    // =========================================================
    // Sound
    // =========================================================

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


    // =========================================================
    // Generate Question
    // =========================================================

    void GenerateQuestion()
    {
        openedWrongBox = false;

        currentAnswer =
            boxMatchManager.CreateNextMatch();

        PlaySfx(
            spawnBoxSfx,
            spawnBoxVolume
        );

        StartCoroutine(
            nextQuestionDelay()
        );

        Debug.Log("==========================");
        Debug.Log("새 문제 생성");
        Debug.Log($"정답 : {currentAnswer}");
    }


    private IEnumerator nextQuestionDelay()
    {
        yield return new WaitForSeconds(0.5f);

        boxButtons.SetActive(true);
    }


    // =========================================================
    // Box Selection
    // =========================================================

    public void SelectUnopened()
    {
        CheckAnswer(
            InspectionResult.Unopened
        );

        boxButtons.SetActive(false);
    }


    public void SelectOpened()
    {
        PlaySfx(
            openBoxSfx,
            openBoxVolume
        );

        boxButtons.SetActive(false);

        // 미개봉 박스를 잘못 열었는지 기록
        openedWrongBox =
            currentAnswer == InspectionResult.Unopened;

        // 박스 제거
        boxMatchManager.RemoveCurrentMatch();

        // 모터 생성
        currentAnswer =
            motorMatchManager.CreateNextMatch();

        Debug.Log(
            $"박스 개봉 완료 / 잘못 연 박스: {openedWrongBox}"
        );

        Debug.Log(
            $"내부 정답: {currentAnswer}"
        );

        // 모터 선택 가능
        motorButtons.SetActive(true);
    }


    // =========================================================
    // Motor Selection
    // =========================================================

    public void SelectA()
    {
        motorButtons.SetActive(false);

        CheckAnswer(
            InspectionResult.A
        );

        motorMatchManager.RemoveCurrentMotor();
    }


    public void SelectB()
    {
        motorButtons.SetActive(false);

        CheckAnswer(
            InspectionResult.B
        );

        motorMatchManager.RemoveCurrentMotor();
    }


    public void SelectC()
    {
        motorButtons.SetActive(false);

        CheckAnswer(
            InspectionResult.C
        );

        motorMatchManager.RemoveCurrentMotor();
    }


    public void SelectDiscard()
    {
        motorButtons.SetActive(false);

        CheckAnswer(
            InspectionResult.Discard
        );

        motorMatchManager.RemoveCurrentMotor();
    }


    // =========================================================
    // Answer Check
    // =========================================================

    void CheckAnswer(
    InspectionResult playerAnswer
)
    {
        bool correct;

        // =====================================================
        // 정답 판정
        // =====================================================

        // 미개봉 박스를 잘못 열었다면
        // 모터 정답과 상관없이 무조건 오답
        if (openedWrongBox)
        {
            correct = false;

            Debug.Log(
                "미개봉 박스를 개봉함 → 모터 선택과 관계없이 오답"
            );
        }
        else
        {
            correct =
                playerAnswer == currentAnswer;
        }


        Debug.Log(
            $"선택 : {playerAnswer}"
        );

        Debug.Log(
            $"정답 : {currentAnswer}"
        );


        // =====================================================
        // 정답 / 오답 처리
        // =====================================================

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

            int reward =
                GetReward(currentAnswer);

            PlayerStatus.Instance
                .AddEarnings(reward);

            Debug.Log(
                $"정답! +{reward}"
            );

            Debug.Log(
                $"현재 콤보 : {comboCount}"
            );


            // =================================================
            // 3콤보 달성
            // =================================================

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
        }
        else
        {
            PlaySfx(
                wrongSfx,
                wrongVolume
            );

            // 오답 개수 증가
            wrongCount++;
            PlayerStatus.Instance.mistakeNumber++;

            // 콤보 초기화
            comboCount = 0;

            PlayerStatus.Instance
                .AddTrustChanges(
                    -trustPenalty
                );

            Debug.Log(
                $"오답! 신뢰도 -{trustPenalty}"
            );

            Debug.Log(
                "콤보가 초기화되었습니다."
            );
        }


        // =====================================================
        // 연료
        // =====================================================

        PlayerStatus.Instance
            .AddFuel(-fuelCost);

        Debug.Log(
            $"연료 -{fuelCost}"
        );


        // =====================================================
        // 통계
        // =====================================================

        Debug.Log(
            $"[검사 통계] 정답: {correctCount} / 오답: {wrongCount} / 현재 콤보: {comboCount}"
        );


        // =====================================================
        // Game Over
        // =====================================================

        if (PlayerStatus.Instance.trust <= 0)
        {
            Debug.Log(
                "GAME OVER - 신뢰도 부족"
            );

            return;
        }

        if (PlayerStatus.Instance.fuel <= 0)
        {
            Debug.Log(
                "GAME OVER - 연료 부족"
            );

            return;
        }


        // =====================================================
        // 다음 문제
        // =====================================================

        GenerateQuestion();
    }


    // =========================================================
    // Reward
    // =========================================================

    int GetReward(
        InspectionResult result
    )
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