using UnityEngine;
using System.Collections;

public class BlackMarketTransition : MonoBehaviour
{
    [Header("To Black Market")]
    [SerializeField] private GoToThisScene goToThisScene;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName = "Knock";

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip knockSound;
    [SerializeField] private AudioClip doorOpenSound;

    [Header("Timing")]
    [SerializeField] private float startDelay = 3f;
    [SerializeField] private float afterKnock= 0.5f;
    [SerializeField] private float animationTime = 5f;

    private void Start()
    {
        StartCoroutine(DoorSequence());
    }

    private IEnumerator DoorSequence()
    {
        // 1. 씬 시작 후 3초 대기
        yield return new WaitForSeconds(startDelay);

        // 2. 노크 효과음 재생
        if (audioSource != null && knockSound != null)
        {
            audioSource.PlayOneShot(knockSound);
        }

        // 3. 노크 후 잠깐 대기
        yield return new WaitForSeconds(afterKnock);

        // 4. 문 열리는 애니메이션 재생
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(animationTime);

        // 5. 문 열리는 효과음 재생
        if (audioSource != null && doorOpenSound != null)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }

        // 7. 기존 GoToThisScene의 nextSceneButton() 호출
        if (goToThisScene != null)
        {
            goToThisScene.nextSceneButton();
        }
        else
        {
            Debug.LogWarning("GoToThisScene이 연결되지 않았습니다.");
        }
    }
}