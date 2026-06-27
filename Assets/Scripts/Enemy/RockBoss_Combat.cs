using UnityEngine;

public class RockBoss_Combat : MonoBehaviour
{
    [Header("Combat Ranges")]
    public float swipeRange = 0.8f;   // Tầm đánh thường (gần)
    public float stompRange = 2f;     // Tầm đánh AOE (dậm đất)
    
    [Header("Cooldowns")]
    public float swipeCooldown = 1f;
    public float stompCooldown = 1f;

    [Header("Damage Settings")]
    public int swipeDamage = 6;
    public int stompDamage = 4;
    
    [Header("Knockback Settings")]
    public float swipeKnockbackForce = 8f;
    public float stompKnockbackForce = 12f;
    public float stunTime = 0.3f;

    [Header("Stomp Debuff Settings")]
    public float stompSlowDuration = 2f;    // Thời gian slow sau khi dậm đất
    public float stompSlowAmount = 0.5f;    // Giảm 50% tốc độ player

    [Header("Enrage Settings (HP < 50%)")]
    public float enrageSpeedMultiplier = 1.4f;    // Tăng 40% tốc độ di chuyển
    public float enrageCooldownMultiplier = 0.6f;  // Giảm 40% thời gian hồi chiêu
    public Color enrageColor = new Color(1f, 0.2f, 0.2f, 1f); // Đỏ khi enrage

    [Header("References")]
    public LayerMask playerLayer;
    
    private float swipeCooldownTimer;
    private float stompCooldownTimer;

    private Animator anim;
    private RockBoss_Movement movementScript;
    private Enemy_Health healthScript;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private bool isEnraged = false;
    private float baseSpeed;
    private Color originalColor;

    void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<RockBoss_Movement>();
        healthScript = GetComponent<Enemy_Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        baseSpeed = movementScript.speed;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        // Cập nhật hồi chiêu
        if (swipeCooldownTimer > 0) swipeCooldownTimer -= Time.deltaTime;
        if (stompCooldownTimer > 0) stompCooldownTimer -= Time.deltaTime;

        // Kiểm tra Enrage Phase
        CheckEnrage();

        EnemyState currentState = movementScript.GetCurrentState();
        player = movementScript.GetPlayerTransform();

        if (player != null && currentState != EnemyState.Knockback && currentState != EnemyState.Attacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= swipeRange && swipeCooldownTimer <= 0)
            {
                PerformSwipe();
            }
            else if (distanceToPlayer <= stompRange && stompCooldownTimer <= 0)
            {
                PerformStomp();
            }
        }
    }

    // ===== ENRAGE PHASE =====
    private void CheckEnrage()
    {
        if (isEnraged || healthScript == null) return;

        // Kích hoạt khi HP dưới 50%
        if (healthScript.currentHealth <= healthScript.maxHealth * 0.5f)
        {
            isEnraged = true;

            // Tăng tốc độ di chuyển
            movementScript.speed = baseSpeed * enrageSpeedMultiplier;

            // Đổi màu đỏ để báo hiệu nguy hiểm
            if (spriteRenderer != null)
            {
                spriteRenderer.color = enrageColor;
            }

            Debug.Log($"<color=red>[ENRAGE]</color> {gameObject.name} đã kích hoạt Enrage! Tốc độ x{enrageSpeedMultiplier}, Cooldown x{enrageCooldownMultiplier}");
        }
    }

    // Lấy cooldown thực tế (có tính enrage)
    private float GetEffectiveCooldown(float baseCooldown)
    {
        return isEnraged ? baseCooldown * enrageCooldownMultiplier : baseCooldown;
    }

    // ===== SWIPE (Đánh tay gần) =====
    private void PerformSwipe()
    {
        movementScript.ChangeState(EnemyState.Attacking);
        swipeCooldownTimer = GetEffectiveCooldown(swipeCooldown);
        anim.SetTrigger("swipeTrig");
        
        Invoke("DealSwipeDamage", 0.25f);
        Invoke("EndAttack", 0.5f);
    }

    // ===== STOMP (Dậm đất AOE) =====
    private void PerformStomp()
    {
        movementScript.ChangeState(EnemyState.Attacking);
        stompCooldownTimer = GetEffectiveCooldown(stompCooldown);
        anim.SetTrigger("stompTrig");

        Invoke("DealStompDamage", 0.4f);
        Invoke("EndAttack", 0.8f);
    }

    public void EndAttack()
    {
        movementScript.FinishAttack();
    }

    public void DealSwipeDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, swipeRange + 0.2f, playerLayer);
        
        foreach (var hit in hits)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ChangeHealth(-swipeDamage);
                hit.GetComponent<PlayerMovement>()?.Knockback(transform, swipeKnockbackForce, stunTime);
                break;
            }
        }
    }

    public void DealStompDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stompRange + 0.2f, playerLayer);
        
        foreach (var hit in hits)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ChangeHealth(-stompDamage);
                hit.GetComponent<PlayerMovement>()?.Knockback(transform, stompKnockbackForce, stunTime);

                // Stomp gây Slow debuff cho player
                Player_DebuffManager debuffManager = hit.GetComponent<Player_DebuffManager>();
                if (debuffManager != null)
                {
                    debuffManager.ApplySlowDebuff(stompSlowDuration, stompSlowAmount);
                }
                break;
            }
        }
    }

    // Hủy mọi Invoke đang pending khi Boss bị disable (chết, bị destroy...)
    // Tránh bug: Boss chết giữa animation nhưng DealDamage vẫn được gọi
    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, swipeRange);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stompRange);
    }
}
