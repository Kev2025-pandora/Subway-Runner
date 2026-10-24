
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public int totalCoins = 0;

    public void AddCoins(int amount)
    {
        // Increase the total coin count
        totalCoins += amount;

        // Update the coin UI
        if (RunnerUI.Instance != null)
        {
            RunnerUI.Instance.SetCoins(totalCoins);
        }
        else
        {
            Debug.LogWarning("RunnerUI instance not found!");
        }

        Debug.Log("Coins collected: " + totalCoins);
    }
}
