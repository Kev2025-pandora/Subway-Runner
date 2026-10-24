
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public bool requiresSlide = false;

    private bool hasHitPlayer = false;

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

        // Find PlayerRunner on the collider or its parent
        PlayerRunner player =
            other.GetComponentInParent<PlayerRunner>();

        if (player == null)
            return;

        Debug.Log("Obstacle detected: " + gameObject.name);

        // Allow passage through slide obstacles while sliding
        if (requiresSlide && player.IsSliding)
        {
            Debug.Log("Slide successful!");
            hasHitPlayer = true;
            return;
        }

        hasHitPlayer = true;

        // Trigger Game Over
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError(
                "GameManager.Instance is null. " +
                "Cannot trigger Game Over."
            );
        }
    }
}
