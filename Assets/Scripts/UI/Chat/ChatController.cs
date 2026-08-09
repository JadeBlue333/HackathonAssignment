using UnityEngine;
using UnityEngine.InputSystem;

public class ChatController : MonoBehaviour
{
    public static ChatController Instance { get; private set; }


    [Header("Chat")]
    [SerializeField]
    private GameObject chatPopUp;


    [Header("Image Change")]
    [SerializeField]
    private GameObject image1;

    [SerializeField]
    private GameObject image2;


    private bool unlocked = false;

    private bool firstChatOpened = false;


    private void Awake()
    {
        Instance = this;

        unlocked = false;
        firstChatOpened = false;
    }


    private void Start()
    {
        if (chatPopUp != null)
        {
            chatPopUp.SetActive(false);
        }

        // 초기 이미지 상태는 Inspector에서
        // Image1 ON / Image2 OFF로 해놔도 됨

        Debug.Log(
            "[ChatController] 시작 / 채팅 잠김"
        );
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (
            Keyboard.current.tKey
                .wasPressedThisFrame
        )
        {
            Debug.Log(
                $"[ChatController] T 입력 / unlocked = {unlocked}"
            );


            if (!unlocked)
                return;


            ToggleChat();
        }
    }


    // =========================================================
    // Unlock
    // =========================================================

    public void UnlockChat()
    {
        unlocked = true;

        Debug.Log(
            "[ChatController] 채팅 해금 완료"
        );
    }


    // =========================================================
    // Toggle
    // =========================================================

    public void ToggleChat()
    {
        if (!unlocked)
            return;


        if (chatPopUp == null)
        {
            Debug.LogError(
                "[ChatController] Chat Pop Up 연결 안됨"
            );

            return;
        }


        bool willOpen =
            !chatPopUp.activeSelf;


        chatPopUp.SetActive(
            willOpen
        );


        // =====================================================
        // 실제로 처음 채팅창을 열었을 때만 이미지 변경
        // =====================================================

        if (
            willOpen &&
            !firstChatOpened
        )
        {
            firstChatOpened = true;


            if (image1 != null)
                image1.SetActive(false);


            if (image2 != null)
                image2.SetActive(true);


            Debug.Log(
                "[ChatController] 첫 채팅 열기 → 이미지 변경"
            );
        }


        Debug.Log(
            $"[ChatController] 채팅창 상태 = {willOpen}"
        );
    }
}