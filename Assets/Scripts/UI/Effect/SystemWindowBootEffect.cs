using System.Collections;
using UnityEngine;

public class SystemWindowBootEffect : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup windowCanvasGroup;

    [SerializeField]
    private float startDelay = 0.3f;

    private void OnEnable()
    {
        StartCoroutine(BootEffect());
    }

    private IEnumerator BootEffect()
    {
        // 처음에는 숨김
        windowCanvasGroup.alpha = 0f;

        yield return new WaitForSeconds(startDelay);


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

        windowCanvasGroup.alpha = 1f;

        // 최종적으로 고정
    }
}