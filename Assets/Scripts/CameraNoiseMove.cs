using UnityEngine;

public class CameraNoiseMove : MonoBehaviour
{
    [Header("Position Noise")]
    [SerializeField] private float positionAmount = 0.02f;
    [SerializeField] private float positionSpeed = 0.5f;

    [Header("Rotation Noise")]
    [SerializeField] private float rotationAmount = 0.15f;
    [SerializeField] private float rotationSpeed = 0.4f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float seedX;
    private float seedY;
    private float seedRot;

    private void Start()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
        seedRot = Random.Range(0f, 100f);
    }

    private void LateUpdate()
    {
        float x =
            (Mathf.PerlinNoise(
                seedX,
                Time.time * positionSpeed
            ) - 0.5f) * 2f;

        float y =
            (Mathf.PerlinNoise(
                seedY,
                Time.time * positionSpeed
            ) - 0.5f) * 2f;

        float rot =
            (Mathf.PerlinNoise(
                seedRot,
                Time.time * rotationSpeed
            ) - 0.5f) * 2f;

        transform.localPosition =
            startPosition +
            new Vector3(
                x * positionAmount,
                y * positionAmount,
                0f
            );

        transform.localRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                0f,
                rot * rotationAmount
            );
    }
}