using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float magnetDuration = 8f;
    public float magnetRadius = 15f;
    public float magnetPullSpeed = 15f;

    private bool isMagnetActive = false;
    private float magnetTimer = 0f;

    public void ActivateMagnet()
    {
        magnetTimer = magnetDuration;
        isMagnetActive = true;

        Debug.Log("Magnet Activated!");
    }

    void Update()
    {
        if (!isMagnetActive)
            return;

        magnetTimer -= Time.deltaTime;

        if (magnetTimer <= 0f)
        {
            isMagnetActive = false;
            Debug.Log("Magnet Ended!");
            return;
        }

        AttractCoins();
    }

    void AttractCoins()
    {
        Coin[] coins =
            FindObjectsByType<Coin>(FindObjectsSortMode.None);

        foreach (Coin coin in coins)
        {
            if (coin == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                coin.transform.position
            );

            if (distance <= magnetRadius)
            {
                coin.transform.position = Vector3.MoveTowards(
                    coin.transform.position,
                    transform.position + Vector3.up,
                    magnetPullSpeed * Time.deltaTime
                );
            }
        }
    }
}