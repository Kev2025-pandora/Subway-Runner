
using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public bool requiresSlide = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 12f;
    public float upwardForce = 6f;
    public float rotationForce = 8f;
    public float destroyDelay = 3f;

    private bool hasHitPlayer = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHitPlayer(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void TryHitPlayer(Collider other)
    {
        if (hasHitPlayer)
            return;

        PlayerRunner player =
            other.GetComponentInParent<PlayerRunner>();

        if (player == null)
            return;

        PlayerGrowth growth =
            player.GetComponent<PlayerGrowth>();

        // Giant player knocks the obstacle away
        if (growth != null && growth.IsGiant)
{
    hasHitPlayer = true;

    ShakeCamera(); // Shake the camera

    Knockback(player.transform);
    return;
}

        // Sliding successfully avoids slide obstacles
        if (requiresSlide && player.IsSliding)
        {
            hasHitPlayer = true;
            return;
        }

        // Normal collision causes Game Over
        hasHitPlayer = true;
        ShakeCamera();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    void Knockback(Transform player)
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        // Calculate the knockback direction
        Vector3 direction =
            (transform.position - player.position).normalized;

        direction.y = 0f;

        Vector3 force =
            direction * knockbackForce +
            Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.Impulse);

        // Add random rotation
        Vector3 torque = new Vector3(
            Random.Range(-rotationForce, rotationForce),
            Random.Range(-rotationForce, rotationForce),
            Random.Range(-rotationForce, rotationForce)
        );

        rb.AddTorque(torque, ForceMode.Impulse);

        Debug.Log("Obstacle knocked away!");

        Destroy(gameObject, destroyDelay);
    }
    void ShakeCamera()
{
    CameraShake shake = Camera.main.GetComponent<CameraShake>();

    if (shake != null)
    {
        shake.Shake(0.4f, 0.3f);
    }
}
}
