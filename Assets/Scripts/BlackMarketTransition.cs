using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BlackMarketTransition : MonoBehaviour
{
    [Header("To Black Market")]
    [SerializeField] private GoToThisScene goToThisScene;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "Knock";

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip knockSound;
    [SerializeField] private AudioClip doorOpenSound;

    [Header("Timing")]
    [SerializeField] private float knockDelay = 1f;
    [SerializeField] private float animationTime = 5f;

    private void Start()
    {
        StartCoroutine(DoorSequence());
    }

    private IEnumerator DoorSequence()
    {
        // 1. 씬 시작 즉시 애니메이션 재생
        if (animator != null)
        {
            animator.Play(animationStateName);
        }

        // 2. 노크 소리까지 대기
        yield return new WaitForSeconds(knockDelay);

        // 3. 노크 소리 재생
        if (audioSource != null && knockSound != null)
        {
            audioSource.PlayOneShot(knockSound);
        }

        // 4. 애니메이션의 남은 시간 대기
        float remainingTime = animationTime - knockDelay;

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // 5. 문 열리는 소리
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        // 6. 다음 씬 이동
        if (goToThisScene != null)
        {
            if (SceneManager.GetActiveScene().name == "BlackMarketTransition")
            {
                if (LanguageManager.Instance.isEnglish)
                {
                    goToThisScene.sceneName = "BlackMarket_EN";
                }
                else
                {
                    goToThisScene.sceneName = "BlackMarket";
                }
            }
                goToThisScene.nextSceneButton();
        }
        else
        {
            Debug.LogWarning("GoToThisScene이 연결되지 않았습니다.");
        }
    }
}