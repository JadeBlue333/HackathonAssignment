using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCameraMouseLook : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform targetCamera;

    [Header("Rotation Amount")]
    [SerializeField] private float horizontalAngle = 3f;
    [SerializeField] private float verticalAngle = 2f;

    [Header("Smooth")]
    [SerializeField] private float smoothSpeed = 5f;

    private Quaternion originalRotation;

    private void Start()
    {
        if (targetCamera != null)
        {
            originalRotation = targetCamera.localRotation;
        }
    }

    private void Update()
    {
        if (targetCamera == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float mouseX = (mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (mousePosition.y / Screen.height - 0.5f) * 2f;

        float yaw = mouseX * horizontalAngle;
        float pitch = -mouseY * verticalAngle;

        Quaternion targetRotation =
            originalRotation * Quaternion.Euler(pitch, yaw, 0f);

        targetCamera.localRotation = Quaternion.Slerp(
            targetCamera.localRotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }
}