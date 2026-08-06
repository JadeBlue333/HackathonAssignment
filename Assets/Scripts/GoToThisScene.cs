using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToThisScene : MonoBehaviour
{
    public string sceneName;
    public Image blackImage;
    public float fadeDuration = 1f;

    [Header("진행도 화면에서 사용할 때만 true로 할 것")]
    public bool progress = false;
    public int buttonDay = 9;

    [Header("정산 화면에서 사용할 때만 true로 할 것.\n정산하고 다음 날 넘어갈때의 변수 이 안에서 고칠 수 있음.")]
    public bool report = false;
    public ReportUI reportUI;

    public void nextSceneButton()
    {
        StartCoroutine(nextScene());
    }

    public IEnumerator nextScene()
    {
        if (progress)
        {
            if (PlayerStatus.Instance.currentDay == buttonDay)
            {
                StartCoroutine(FadeIn());
                yield return new WaitForSeconds(fadeDuration);
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("이미 지난 날입니다.");
            }
        }
        else if (report)
        {
            StartCoroutine(FadeIn());
            yield return new WaitForSeconds(fadeDuration);

            //이부분에서 NextDay()와 돈/신뢰도 offset설정 함수를 호출하여 날짜 바꾸고 그 후에 씬을 로드
            PlayerStatus.Instance.NextDay();
            PlayerStatus.Instance.ApplyEarnings();
            PlayerStatus.Instance.ApplyTrustChanges();
            if (reportUI.fuelToggle.isOn)
            {
                PlayerStatus.Instance.fuel += 30;
                PlayerStatus.Instance.money -= 20;
            }
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            StartCoroutine(FadeIn());
            yield return new WaitForSeconds(fadeDuration);
            SceneManager.LoadScene(sceneName);
        }
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
