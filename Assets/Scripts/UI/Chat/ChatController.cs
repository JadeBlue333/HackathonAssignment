using UnityEngine;
using UnityEngine.InputSystem;

public class ChatController : MonoBehaviour
{
    [Header("Chat")]
    [SerializeField] private GameObject chatPopUp;

    private bool unlocked = false;


    private void Start()
    {
        if (chatPopUp != null)
            chatPopUp.SetActive(false);
    }


    private void Update()
    {
        if (!unlocked)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            ToggleChat();
        }
    }


    public void UnlockChat()
    {
        unlocked = true;

        Debug.Log("채팅 기능 활성화");
    }


    public void ToggleChat()
    {
        if (!unlocked)
            return;

        if (chatPopUp == null)
            return;

        chatPopUp.SetActive(
            !chatPopUp.activeSelf
        );
    }
}