using UnityEngine;
using UnityEngine.SceneManagement;

public class InspectionGameManager : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private int unopenedReward = 3;
    [SerializeField] private int aReward = 2;
    [SerializeField] private int bReward = 1;
    [SerializeField] private int cReward = 1;
    [SerializeField] private int discardReward = 0;

    [Header("Penalty")]
    [SerializeField] private int trustPenalty = 5;
    [SerializeField] private int fuelCost = 2;

    private InspectionResult currentAnswer;

    private void Start()
    {
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        currentAnswer = (InspectionResult)Random.Range(0, 5);

        Debug.Log("=================================");
        Debug.Log($"정답 : {currentAnswer}");
    }

    public void SelectUnopened()
    {
        CheckAnswer(InspectionResult.Unopened);
    }

    public void SelectA()
    {
        CheckAnswer(InspectionResult.A);
    }

    public void SelectB()
    {
        CheckAnswer(InspectionResult.B);
    }

    public void SelectC()
    {
        CheckAnswer(InspectionResult.C);
    }

    public void SelectDiscard()
    {
        CheckAnswer(InspectionResult.Discard);
    }

    void CheckAnswer(InspectionResult playerAnswer)
    {
        bool correct = playerAnswer == currentAnswer;

        if (correct)
        {
            int reward = GetReward(currentAnswer);

            PlayerStatus.Instance.AddMoney(reward);

            Debug.Log($"정답! +{reward} 크레타");
        }
        else
        {
            PlayerStatus.Instance.AddTrust(-trustPenalty);

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