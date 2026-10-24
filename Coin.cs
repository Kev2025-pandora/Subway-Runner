
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed = 180f;
    public int coinValue = 1;

    void Update()
    {
        // Rotate the upright coin around the world Y axis
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // Check whether the player collected the coin
        if (other.GetComponentInParent<PlayerRunner>() != null)
        {
            CoinManager manager =
                FindFirstObjectByType<CoinManager>();

            if (manager != null)
            {
                manager.AddCoins(coinValue);
            }

            Destroy(gameObject);
        }
    }
}
