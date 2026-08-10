using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowProgress : MonoBehaviour
{
    [Header("호버할 버튼")]
    [SerializeField] private Button targetButton;

    [Header("버튼 호버 시 보여줄 텍스트")]
    [SerializeField] private GameObject hoverText;

    private void Start()
    {
        if (hoverText != null)
            hoverText.SetActive(false);

        if (targetButton != null)
        {
            // EventTrigger 추가
            EventTrigger trigger = targetButton.gameObject.GetComponent<EventTrigger>();

            if (trigger == null)
                trigger = targetButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Enter
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => OnButtonHoverEnter());
            trigger.triggers.Add(pointerEnter);

            // Pointer Exit
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => OnButtonHoverExit());
            trigger.triggers.Add(pointerExit);
        }
    }

    private void OnButtonHoverEnter()
    {
        // 버튼이 비활성화 상태면 아무것도 하지 않음
        if (targetButton == null || !targetButton.interactable)
            return;

        if (hoverText != null)
            hoverText.SetActive(true);
    }

    private void OnButtonHoverExit()
    {
        if (hoverText != null)
            hoverText.SetActive(false);
    }

    private void Update()
    {
        // 버튼이 호버 중에 interactable -> false가 되는 경우도 처리
        if (targetButton != null &&
            !targetButton.interactable &&
            hoverText != null &&
            hoverText.activeSelf)
        {
            hoverText.SetActive(false);
        }
    }
}