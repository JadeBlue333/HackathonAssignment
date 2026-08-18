using System.Collections;
using UnityEngine;

public class WindowBootEffect : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    private CanvasGroup windowCanvasGroup;


    [Header("Boot Effect")]
    [SerializeField]
    private float startDelay = 0.3f;


    [Header("Sound")]
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip bootSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float bootSoundVolume = 1f;


    private void OnEnable()
    {
        StartCoroutine(BootEffect());
    }


    private IEnumerator BootEffect()
    {
        if (windowCanvasGroup == null)
            yield break;


        windowCanvasGroup.alpha = 0f;

        yield return new WaitForSeconds(startDelay);


        if (audioSource != null &&
            bootSound != null)
        {
            audioSource.PlayOneShot(
                bootSound,
                bootSoundVolume
            );
        }


        // 첫 번째 점멸
        windowCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.05f);

        windowCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(0.08f);


        // 두 번째 점멸
        windowCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.03f);

        windowCanvasGroup.alpha = 0f;
        yield return new WaitForSeconds(0.05f);


        // 세 번째 점멸
        windowCanvasGroup.alpha = 0.5f;
        yield return new WaitForSeconds(0.04f);


        // 최종 표시
        windowCanvasGroup.alpha = 1f;
    }
}