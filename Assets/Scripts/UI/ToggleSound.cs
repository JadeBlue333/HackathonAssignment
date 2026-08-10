using UnityEngine;
using UnityEngine.UI;

public class ToggleSound : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Toggle toggle;
    [SerializeField] private AudioSource audioSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip checkSound;
    [SerializeField] private AudioClip uncheckSound;

    private void Awake()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (toggle != null)
            toggle.onValueChanged.AddListener(PlayToggleSound);
    }

    private void OnDisable()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(PlayToggleSound);
    }

    private void PlayToggleSound(bool isOn)
    {
        if (audioSource == null)
            return;

        AudioClip clip = isOn ? checkSound : uncheckSound;

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}