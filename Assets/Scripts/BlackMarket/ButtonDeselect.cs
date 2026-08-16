using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDeselect : MonoBehaviour
{
    // Button의 On Click()에 연결할 함수
    public void DeselectButton()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}