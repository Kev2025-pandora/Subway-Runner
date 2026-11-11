
using UnityEngine;
using System.Collections;

public class PlayerGrowth : MonoBehaviour
{
    [Header("Growth Settings")]
    public Transform visualRoot;
    public float growthMultiplier = 2f;
    public float growthDuration = 6f;

    [Header("Speed Settings")]
    public float speedMultiplier = 1.5f;

    [Header("Warning Settings")]
    public int blinkCount = 3;
    public float blinkDuration = 1.5f;

    private Vector3 originalScale;
    private SpeedManager speedManager;
    private Coroutine growthCoroutine;

    private Renderer[] renderers;
    private bool[] originalRendererStates;

    public bool IsGiant { get; private set; }

    void Start()
    {
        if (visualRoot != null)
        {
            originalScale = visualRoot.localScale;

            renderers = visualRoot.GetComponentsInChildren<Renderer>(true);

            originalRendererStates = new bool[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalRendererStates[i] = renderers[i].enabled;
            }
        }
        else
        {
            Debug.LogError("VisualRoot is not assigned!");
        }

        speedManager = FindObjectOfType<SpeedManager>();

        if (speedManager == null)
        {
            Debug.LogError("SpeedManager not found!");
        }
    }

    public void ActivateGrowth()
    {
        if (visualRoot == null || speedManager == null)
            return;

        // Refresh the duration if collected again
        if (growthCoroutine != null)
        {
            StopCoroutine(growthCoroutine);
        }

        RestoreRenderers();

        // Apply the growth effect only once
        if (!IsGiant)
        {
            IsGiant = true;

            visualRoot.localScale =
                originalScale * growthMultiplier;

            speedManager.currentSpeed *= speedMultiplier;
        }

        growthCoroutine = StartCoroutine(GrowthRoutine());
    }

    IEnumerator GrowthRoutine()
    {
        // Wait until the warning period begins
        float warningTime = Mathf.Min(
            blinkDuration,
            growthDuration
        );

        yield return new WaitForSeconds(
            growthDuration - warningTime
        );

        // Blink three times before shrinking
        float interval = warningTime / (blinkCount * 2f);

        for (int i = 0; i < blinkCount; i++)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(interval);

            SetRenderersVisible(true);
            yield return new WaitForSeconds(interval);
        }

        // Restore original appearance and speed
        RestoreRenderers();

        visualRoot.localScale = originalScale;

        speedManager.currentSpeed /= speedMultiplier;

        IsGiant = false;
        growthCoroutine = null;
    }

    void SetRenderersVisible(bool visible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled =
                    visible && originalRendererStates[i];
            }
        }
    }

    void RestoreRenderers()
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled =
                    originalRendererStates[i];
            }
        }
    }
}
