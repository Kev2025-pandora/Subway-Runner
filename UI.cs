
using TMPro;
using UnityEngine;

public class RunnerUI : MonoBehaviour
{
    public static RunnerUI Instance;

    [Header("UI Text")]
    public TMP_Text coinText;
    public TMP_Text scoreText;

    [Header("Score Settings")]
    public Transform player;
    public float scoreMultiplier = 1f;

    private int score = 0;
    private int coins = 0;
    private float startZ;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Record the player's starting position
        if (player != null)
        {
            startZ = player.position.z;
        }
        else
        {
            Debug.LogError("RunnerUI: Player is not assigned!");
        }

        UpdateCoinUI();
        UpdateScoreUI();
    }

    void Update()
    {
        if (player == null)
            return;

        // Calculate score based on forward distance
        score = Mathf.Max(
            0,
            Mathf.FloorToInt(
                (player.position.z - startZ) * scoreMultiplier
            )
        );

        UpdateScoreUI();
    }

    public void SetCoins(int totalCoins)
    {
        coins = totalCoins;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
