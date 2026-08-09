using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler
{
    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("마우스 호버 시 재생")]
    [SerializeField] private AudioClip hoverSound;

    [Tooltip("버튼 클릭 시 재생")]
    [SerializeField] private AudioClip clickSound;


    private void Awake()
    {
        // 안 넣었으면 자기 자신의 Button 자동으로 찾음
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }


    // =========================================================
    // Hover
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 비활성화 버튼이면 소리 안 남
        if (button != null && !button.interactable)
            return;

        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }


    // =========================================================
    // Click
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        // 비활성화 버튼이면 소리 안 남
        if (button != null && !button.interactable)
            return;

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}