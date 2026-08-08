using UnityEngine;

public class MoneyTest : MonoBehaviour
{
    // 오늘 수익 +10
    public void AddEarnings10()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        PlayerStatus.Instance.AddEarnings(10);

        Debug.Log(
            $"수익 +10 / 보유금: {PlayerStatus.Instance.money} / 오늘 수익: {PlayerStatus.Instance.earnings}"
        );
    }

    // 오늘 수익 +20
    public void AddEarnings20()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        PlayerStatus.Instance.AddEarnings(20);

        Debug.Log(
            $"수익 +20 / 보유금: {PlayerStatus.Instance.money} / 오늘 수익: {PlayerStatus.Instance.earnings}"
        );
    }

    // 오늘 번 돈을 실제 보유금에 반영
    public void ApplyEarnings()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        PlayerStatus.Instance.ApplyEarnings();

        Debug.Log(
            $"수익 정산 완료 / 현재 보유금: {PlayerStatus.Instance.money} / 오늘 수익: {PlayerStatus.Instance.earnings}"
        );
    }

    // 돈 5 사용
    public void SpendMoney5()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        bool success = PlayerStatus.Instance.SpendMoney(5);

        Debug.Log(
            $"5원 사용 시도 / 성공 여부: {success} / 현재 보유금: {PlayerStatus.Instance.money}"
        );
    }

    // 돈 10 사용
    public void SpendMoney10()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        bool success = PlayerStatus.Instance.SpendMoney(10);

        Debug.Log(
            $"10원 사용 시도 / 성공 여부: {success} / 현재 보유금: {PlayerStatus.Instance.money}"
        );
    }

    // 돈 20 사용
    public void SpendMoney20()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        bool success = PlayerStatus.Instance.SpendMoney(20);

        Debug.Log(
            $"20원 사용 시도 / 성공 여부: {success} / 현재 보유금: {PlayerStatus.Instance.money}"
        );
    }

    // 현재 돈 상태 확인
    public void PrintMoneyStatus()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        Debug.Log(
            $"현재 보유금: {PlayerStatus.Instance.money} / 오늘 수익: {PlayerStatus.Instance.earnings}"
        );
    }

    // 테스트용으로 돈 +10 즉시 추가
    public void AddMoney10()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        PlayerStatus.Instance.AddMoney(10);

        Debug.Log(
            $"보유금 즉시 +10 / 현재 보유금: {PlayerStatus.Instance.money}"
        );
    }

    // 테스트용으로 돈 초기값 20으로 되돌리기
    public void ResetMoneyTest()
    {
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus가 없습니다.");
            return;
        }

        PlayerStatus.Instance.money = 20;
        PlayerStatus.Instance.earnings = 0;

        Debug.Log("돈 테스트 초기화 / 보유금: 20 / 오늘 수익: 0");
    }
}