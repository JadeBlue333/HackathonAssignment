using TMPro;
using UnityEngine;

public class SceneTextState : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Header("Alpha")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.2f;

    public void SetActiveState(bool isActive)
    {
        Color c = text.color;

        c.a = isActive ? activeAlpha : inactiveAlpha;

        text.color = c;
    }
}