using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToThisScene : MonoBehaviour
{
    public string sceneName;
    public Image blackImage;
    public float fadeDuration = 1f;

    public void nextSceneButton()
    {
        StartCoroutine(nextScene());
    }

    public IEnumerator nextScene()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        Color color = blackImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            blackImage.color = color;
            yield return null;
        }

        color.a = 1;
        blackImage.color = color;
    }
}
