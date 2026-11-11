using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [Header("Speed Settings")]
    public float startSpeed = 8f;

    public float fastAcceleration = 1.2f;
    public float slowAcceleration = 0.3f;

    public float transitionSpeed = 25f;
    public float currentSpeed;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    void Update()
    {
        float acceleration;

        if (currentSpeed < transitionSpeed)
        {
            acceleration = fastAcceleration;
        }
        else
        {
            float difference = currentSpeed - transitionSpeed;

            acceleration = slowAcceleration /
                (1f + difference * 0.15f);
        }

        currentSpeed += acceleration * Time.deltaTime;
    }
}