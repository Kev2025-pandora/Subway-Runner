using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        MagnetEffect magnet =
            other.GetComponentInParent<MagnetEffect>();

        if (magnet != null)
        {
            magnet.ActivateMagnet();
            Destroy(gameObject);
        }
    }
}