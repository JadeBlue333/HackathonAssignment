using UnityEngine;
using UnityEngine.EventSystems;

public class RotateOnHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Rotation Target")]
    [Tooltip("실제로 회전할 톱니바퀴 오브젝트")]
    [SerializeField] private RectTransform rotationTarget;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 120f;


    [Header("Hover Audio")]
    [SerializeField] private AudioSource hoverAudioSource;
    [SerializeField] private AudioClip hoverSound;

    [Range(0f, 1f)]
    [SerializeField] private float hoverVolume = 1f;


    [Header("Click Audio")]
    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioClip clickSound;

    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 1f;


    private bool isHovered = false;


    private void Awake()
    {
        if (hoverAudioSource != null)
        {
            hoverAudioSource.playOnAwake = false;
            hoverAudioSource.loop = true;
        }

        if (clickAudioSource != null)
        {
            clickAudioSource.playOnAwake = false;
            clickAudioSource.loop = false;
        }
    }


    private void Update()
    {
        if (!isHovered)
            return;

        if (rotationTarget == null)
            return;

        // 지정한 톱니바퀴만 회전
        rotationTarget.Rotate(
            0f,
            0f,
            -rotationSpeed * Time.unscaledDeltaTime
        );
    }


    // =========================================================
    // Hover Start
    // =========================================================

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        if (hoverAudioSource != null &&
            hoverSound != null)
        {
            hoverAudioSource.clip = hoverSound;
            hoverAudioSource.volume = hoverVolume;

            if (!hoverAudioSource.isPlaying)
            {
                hoverAudioSource.Play();
            }
        }
    }


    // =========================================================
    // Hover End
    // =========================================================

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (hoverAudioSource != null)
        {
            hoverAudioSource.Stop();
        }
    }


    // =========================================================
    // Click
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickAudioSource != null &&
            clickSound != null)
        {
            clickAudioSource.PlayOneShot(
                clickSound,
                clickVolume
            );
        }
    }


    private void OnDisable()
    {
        isHovered = false;

        if (hoverAudioSource != null)
        {
            hoverAudioSource.Stop();
        }
    }
}