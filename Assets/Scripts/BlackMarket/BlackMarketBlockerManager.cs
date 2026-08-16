using UnityEngine;
using UnityEngine.UI;

public class BlackMarketBlockerManager : MonoBehaviour
{
    [Header("판매 패널 블로커")]

    [Tooltip("신뢰도 1단계부터 잠길 패널")]
    [SerializeField] private Image blocker1;

    [Tooltip("신뢰도 2단계부터 추가로 잠길 패널")]
    [SerializeField] private Image blocker2;

    [Tooltip("신뢰도 3단계부터 추가로 잠길 패널")]
    [SerializeField] private Image blocker3;

    [Header("테스트용 설정")]

    [Tooltip("PlayerStatus가 없을 때 사용할 임시 신뢰도")]
    [Range(0, 100)]
    [SerializeField] private int testTrust = 50;


    private void OnEnable()
    {
        // 암시장 UI가 켜질 때마다 현재 신뢰도 기준으로 갱신
        RefreshBlockers();
    }


    public void RefreshBlockers()
    {
        //여기서부터 테스트 코드 =============================================================
        //int currentTrust;

        // 실제 PlayerStatus가 존재하면 실제 데이터 사용
        //if (PlayerStatus.Instance != null)
        //{
        //    currentTrust = PlayerStatus.Instance.trust;
        //}
        //// PlayerStatus가 없으면 Inspector의 테스트 값 사용
        //else
        //{
        //    currentTrust = testTrust;

        //    Debug.LogWarning(
        //        $"PlayerStatus가 없습니다. 테스트 신뢰도 {testTrust} 사용"
        //    );
        //}
        //여기까지 테스트코드 ================================================================
        //실제 플레이시 코드 아래로 교체
        if (PlayerStatus.Instance == null)
        {
            Debug.LogWarning("PlayerStatus.Instance를 찾을 수 없습니다.");
            return;
        }

        int currentTrust = PlayerStatus.Instance.trust;
        //여기까지 ======================================================================= 요 한칸 활성화하고 테스트코드 비활 ㄱ

        // ================================================
        // 신뢰도 단계별 블로커 설정
        // ================================================

        if (currentTrust >= 90)
        {
            // 신뢰도 높음
            // 판매 패널 3개 모두 잠금
            SetBlocker(blocker1, true);
            SetBlocker(blocker2, true);
            SetBlocker(blocker3, true);
        }
        else if (currentTrust >= 70)
        {
            // 신뢰도 중간
            // 판매 패널 2개 잠금
            SetBlocker(blocker1, true);
            SetBlocker(blocker2, true);
            SetBlocker(blocker3, false);
        }
        else if (currentTrust >= 50)
        {
            // 신뢰도 낮음
            // 판매 패널 1개만 잠금
            SetBlocker(blocker1, true);
            SetBlocker(blocker2, false);
            SetBlocker(blocker3, false);
        }
        else
        {
            SetBlocker(blocker1, false);
            SetBlocker(blocker2, false);
            SetBlocker(blocker3, false);
        }

        Debug.Log($"현재 신뢰도 : {currentTrust}");
    }


    // 블로커 활성화 / 비활성화
    private void SetBlocker(Image blocker, bool active)
    {
        if (blocker == null)
            return;

        blocker.gameObject.SetActive(active);
    }
}