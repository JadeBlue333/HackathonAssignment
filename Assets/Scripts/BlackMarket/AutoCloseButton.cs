using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AutoCloseButton : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private float closeDelay = 5f;

    public async void StartAutoClose()
    {
        int delayMilliseconds = Mathf.RoundToInt(closeDelay * 1000f);

        await Task.Delay(delayMilliseconds);

        if (closeButton != null)
        {
            closeButton.onClick.Invoke();
        }
    }
}