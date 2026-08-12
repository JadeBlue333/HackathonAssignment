using UnityEngine;
using UnityEngine.UI;

public class ShowEndings : MonoBehaviour
{
    public Image ending1;
    public Image ending2;
    public Image ending3;
    public Image ending4;
    public Image ending5;

    void Start()
    {
        if (PlayerStatus.Instance != null)
        {
            SetEndingAlpha(ending1, PlayerStatus.Instance.ending1Achieved);
            SetEndingAlpha(ending2, PlayerStatus.Instance.ending2Achieved);
            SetEndingAlpha(ending3, PlayerStatus.Instance.ending3Achieved);
            SetEndingAlpha(ending4, PlayerStatus.Instance.ending4Achieved);
            SetEndingAlpha(ending5, PlayerStatus.Instance.ending5Achieved);
        }
    }

    private void SetEndingAlpha(Image image, bool achieved)
    {
        Color color = image.color;
        color.a = achieved ? 1f : 0.2f;
        image.color = color;
    }
}