using UnityEngine;

public class CameraVerticalScroller : MonoBehaviour
{
    public float startSpeed = 1f;
    public float maxSpeed = 5f;
    public float acceleration = 0.2f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        // Aumentar velocidad progresivamente
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

        // Mover la cámara hacia arriba
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }
}