using System.Collections;
using UnityEngine;

public class Enemy_DebuffManager : MonoBehaviour
{
    [Header("Enemy Type Settings")]
    public bool isBoss = false;

    [Header("Visual Effects")]

    public Color freezeColor = new Color(0.3f, 0.75f, 1f, 1f); 
    public Color slowColor = new Color(0.6f, 0.4f, 0.8f, 1f);
    public Color burnColor = new Color(1f, 0.3f, 0f, 1f);
    private Color originalColor;            

    private Torch_Movement torchMovement;
    private Archer_Movement archerMovement;
    private Barrel_Suicide barrelSuicide;
    private Frostbite_Archer_Movement frostbiteArcherMovement;

    private Enemy_Health enemyHealth;
    private SpriteRenderer spriteRenderer; 

    private float originalSpeed;
    private Coroutine activeDebuffRoutine;
    private Coroutine activeBurnRoutine;

    private void Awake()
    {
        
        torchMovement = GetComponent<Torch_Movement>();
        archerMovement = GetComponent<Archer_Movement>();
        barrelSuicide = GetComponent<Barrel_Suicide>();
        frostbiteArcherMovement = GetComponent<Frostbite_Archer_Movement>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<Enemy_Health>();

        if (torchMovement != null) originalSpeed = torchMovement.speed;
        if (archerMovement != null) originalSpeed = archerMovement.speed;
        if (barrelSuicide != null) originalSpeed = barrelSuicide.speed;
        if (frostbiteArcherMovement != null) originalSpeed = frostbiteArcherMovement.speed;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            originalColor.a = 1f;
        }
        /*else
        {
            originalColor = Color.white;
        }*/
    }

    public void ApplyArrowDebuff()
    {
        if (activeDebuffRoutine != null)
        {
            float burnRoll = Random.value;
            if (burnRoll <= StatsManager.Instance.arrowBurnChance)
            {
                ResetActiveDebuff();
                UpdateVisualColor();
                Debug.Log($"<color=orange>[CONFLICT]</color>Fire has melted the Ice on {gameObject.name}!");
            }
            return; 
        }

        if (activeBurnRoutine != null)
        {
            float freezeRoll = Random.value;
            if ((!isBoss && freezeRoll <= StatsManager.Instance.arrowFreezeChance) || freezeRoll <= StatsManager.Instance.arrowSlowChance)
            {
                StopCoroutine(activeBurnRoutine);
                activeBurnRoutine = null;
                UpdateVisualColor();
                Debug.Log($"<color=cyan>[CONFLICT]</color> Ice/Slow has extinguished Fire on {gameObject.name}!");
            }
            return;
        }

        float roll = Random.value;

        if (!isBoss && roll <= StatsManager.Instance.arrowFreezeChance)
        {
            ResetActiveDebuff();
            activeDebuffRoutine = StartCoroutine(ApplySlowRoutine(
                freezeDuration: StatsManager.Instance.arrowFreezeDuration,
                slowDuration: 1f,
                slowAmount: StatsManager.Instance.arrowSlowAmount
            ));
            UpdateVisualColor();
            return;
        }
        else if (roll <= StatsManager.Instance.arrowSlowChance)
        {
            ResetActiveDebuff();
            activeDebuffRoutine = StartCoroutine(ApplySlowRoutine(
                freezeDuration: 0f,
                slowDuration: StatsManager.Instance.arrowSlowDuration,
                slowAmount: StatsManager.Instance.arrowSlowAmount
            ));
            UpdateVisualColor();
            return;
        }

        float standaloneBurnRoll = Random.value;
        if (standaloneBurnRoll <= StatsManager.Instance.arrowBurnChance)
        {
            if (activeBurnRoutine != null) StopCoroutine(activeBurnRoutine);
            activeBurnRoutine = StartCoroutine(BurnRoutine(StatsManager.Instance.arrowBurnDuration, StatsManager.Instance.arrowBurnDamage));
            UpdateVisualColor();
        }
    }

    private void ResetActiveDebuff()
    {
        if (activeDebuffRoutine != null)
        {
            StopCoroutine(activeDebuffRoutine);
        }
        SetEnemySpeed(originalSpeed);
        SetAnimatorSpeed(1f);
        
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    // Coroutine for freeze and slow debuffs
    private IEnumerator ApplySlowRoutine(float freezeDuration, float slowDuration, float slowAmount)
    {
        if (freezeDuration > 0f)
        {
            SetEnemySpeed(0f);
            SetAnimatorSpeed(0f);

            if (spriteRenderer != null) spriteRenderer.color = freezeColor;

            yield return new WaitForSeconds(freezeDuration);
        }

        if (slowDuration > 0f)
        {
            float slowedSpeed = originalSpeed * (1f - slowAmount);
            SetEnemySpeed(slowedSpeed);
            SetAnimatorSpeed(1f);
            if (spriteRenderer != null) spriteRenderer.color = slowColor;

            yield return new WaitForSeconds(slowDuration);
        }

        SetEnemySpeed(originalSpeed);
        SetAnimatorSpeed(1f);
        activeDebuffRoutine = null;
        UpdateVisualColor();
    }

    // Coroutine for burn DoT debuff
    private IEnumerator BurnRoutine(float duration, float damagePerSecond)
    {
        float totalTimer = duration;
        float damageTickTimer = 1f;

        UpdateVisualColor();

        while (totalTimer > 0f)
        {
            float deltaTime = Time.deltaTime;
            totalTimer -= deltaTime;
            damageTickTimer -= deltaTime;

            if (damageTickTimer <= 0f)
            {
                if (enemyHealth != null)
                {
                    enemyHealth.ChangeHealth(-damagePerSecond);
                    // Debug.Log($"<color=red>[BURN LOG]</color> Inflicts {damagePerSecond} DoT on {gameObject.name}. Current Health: {enemyHealth.currentHealth}, Remaining Burn Time: {Mathf.Max(0f, totalTimer):F1}s");
                }

                damageTickTimer = 1f; 
                UpdateVisualColor();
            }

            yield return null;
        }

        activeBurnRoutine = null;
        UpdateVisualColor();
        //Debug.Log($"<color=yellow>[BURN END]</color>.");
    }
    private void UpdateVisualColor()
    {
        if (spriteRenderer == null) return;

        if (activeBurnRoutine != null)
        {
            spriteRenderer.color = burnColor; 
        }
        else if (activeDebuffRoutine != null)
        {
            spriteRenderer.color = (torchMovement?.speed == 0f || archerMovement?.speed == 0f || barrelSuicide?.speed == 0f) ? freezeColor : slowColor;
        }
        else
        {
            spriteRenderer.color = originalColor; 
        }
    }

    private void SetEnemySpeed(float newSpeed)
    {
        if (torchMovement != null) torchMovement.speed = newSpeed;
        if (archerMovement != null) archerMovement.speed = newSpeed;
        if (barrelSuicide != null) barrelSuicide.speed = newSpeed;
        if (frostbiteArcherMovement != null) frostbiteArcherMovement.speed = newSpeed;
    }

    private void SetAnimatorSpeed(float animSpeed)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = animSpeed;
        }
    }

    private void OnDestroy()
    {
        if (activeDebuffRoutine != null) StopCoroutine(activeDebuffRoutine);
        if (activeBurnRoutine != null) StopCoroutine(activeBurnRoutine);
    }
}