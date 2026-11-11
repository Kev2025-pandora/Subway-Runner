using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    public SpeedManager speedManager;
    public TMP_Text speedText;

    void Update()
    {
        if (speedManager == null || speedText == null)
            return;

        speedText.text =
            "SPEED: " +
            speedManager.currentSpeed.ToString("F1") +
            " m/s";
    }
}