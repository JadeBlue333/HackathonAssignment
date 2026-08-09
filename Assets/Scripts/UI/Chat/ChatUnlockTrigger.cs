using UnityEngine;

public class ChatUnlockTrigger : MonoBehaviour
{
    private bool triggered = false;


    private void OnEnable()
    {
        if (triggered)
            return;


        triggered = true;


        if (ChatController.Instance == null)
        {
            Debug.LogError(
                "[ChatUnlockTrigger] ChatController.Instance가 없습니다."
            );

            return;
        }


        ChatController.Instance.UnlockChat();


        Debug.Log(
            $"[ChatUnlockTrigger] {gameObject.name} 등장 → 채팅 해금"
        );
    }
}