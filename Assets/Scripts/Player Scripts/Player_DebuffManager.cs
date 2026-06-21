using System.Collections;
using UnityEngine;

public class Player_DebuffManager : MonoBehaviour
{
    [Header("Visual Effects")]
    public Color burnColor = new Color(1f, 0.3f, 0f, 1f);
    public Color slowColor = new Color(0.3f, 0.75f, 1f, 1f);
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    private Coroutine activeBurnRoutine;
    private Coroutine activeSlowRoutine;
    private float baseSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            originalColor.a = 1f;
        }
    }

    // Apply Burn Debuff
    public void ApplyBurnDebuff(float duration, float damagePerSecond)
    {
        if (activeBurnRoutine != null)
        {
            StopCoroutine(activeBurnRoutine);
        }
        activeBurnRoutine = StartCoroutine(BurnRoutine(duration, damagePerSecond));
    }

    private IEnumerator BurnRoutine(float duration, float damagePerSecond)
    {
        float totalTimer = duration;
        float damageTickTimer = 1f;

        if (spriteRenderer != null) spriteRenderer.color = burnColor;

        while (totalTimer > 0f)
        {
            float deltaTime = Time.deltaTime;
            totalTimer -= deltaTime;
            damageTickTimer -= deltaTime;

            if (damageTickTimer <= 0f)
            {
                if (playerHealth != null)
                {
                    playerHealth.ChangeHealth(-damagePerSecond);
                }
                damageTickTimer = 1f;
            }
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        activeBurnRoutine = null;
        UpdateVisualColor();
    }

    // Apply Slow Debuff
    public void ApplySlowDebuff(float duration, float slowAmount)
    {
        if (activeSlowRoutine == null && StatsManager.Instance != null)
        {
            baseSpeed = StatsManager.Instance.speed;
        }
        else if (activeSlowRoutine != null)
        {
            StopCoroutine(activeSlowRoutine);
        }

        activeSlowRoutine = StartCoroutine(SlowRoutine(duration, slowAmount));
    }

    private IEnumerator SlowRoutine(float duration, float slowAmount)
    {
        if (playerMovement != null && StatsManager.Instance != null)
        {
            StatsManager.Instance.speed = baseSpeed * (1f - slowAmount);
            if (StatsManager.Instance.statsUI != null) StatsManager.Instance.statsUI.UpdateAllStats();
        }

        UpdateVisualColor();

        yield return new WaitForSeconds(duration);

        if (playerMovement != null && StatsManager.Instance != null)
        {
            StatsManager.Instance.speed = baseSpeed;
            if (StatsManager.Instance.statsUI != null) StatsManager.Instance.statsUI.UpdateAllStats();
        }

        activeSlowRoutine = null;
        UpdateVisualColor();
    }

    private void UpdateVisualColor()
    {
        if (spriteRenderer == null) return;

        if (activeBurnRoutine != null) spriteRenderer.color = burnColor;
        else if (activeSlowRoutine != null) spriteRenderer.color = slowColor;
        else spriteRenderer.color = originalColor;
    }

    public void ResetAllDebuffs()
    {
        if (activeBurnRoutine != null)
        {
            StopCoroutine(activeBurnRoutine);
            activeBurnRoutine = null;
        }

        if (activeSlowRoutine != null)
        {
            StopCoroutine(activeSlowRoutine);
            activeSlowRoutine = null;
            if (StatsManager.Instance != null && baseSpeed > 0)
            {
                StatsManager.Instance.speed = baseSpeed;
                if (StatsManager.Instance.statsUI != null) StatsManager.Instance.statsUI.UpdateAllStats();
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private void OnDestroy()
    {
        ResetAllDebuffs();
    }
}