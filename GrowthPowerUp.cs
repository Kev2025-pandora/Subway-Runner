
using UnityEngine;

public class GrowthPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerGrowth player =
            other.GetComponentInParent<PlayerGrowth>();

        if (player != null)
        {
            player.ActivateGrowth();

            Destroy(gameObject);
        }
    }
}
