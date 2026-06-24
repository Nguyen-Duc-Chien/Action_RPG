using System.Collections;
using UnityEngine;

public enum BarrelState { Idle, Awakening, Chasing, Retracting, Igniting, Knockback }

public class Barrel_Suicide : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    [Tooltip("Round 1: Detection range to trigger alert (Green Gizmo)")]
    public float playerDetectionRange = 5f;
    [Tooltip("Round 2: Range begins chasing the player (Gizmo Blue)")]
    public float chaseTriggerRange = 3.2f;
    [Tooltip("Round 3: Approach to start the detonation countdown (Golden Gizmo)")]
    public float explodeTriggerRange = 2f;

    [Header("Explosion Settings")]
    public float maxExplosionRadius = 2.5f; // Damage radius (Red Gizmo)
    public float maxDamage = 35f;
    public float minDamage = 10f;
    public float knockbackForce = 10f;
    public float stunTime = 0.3f;
    public float fuseDuration = 1f;
    public LayerMask playerLayer;

    private BarrelState currentState;
    private float fuseTimer;
    private float awakenTimer; 
    private int facingDirection = -1;
    private bool isExploded = false;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        ChangeState(BarrelState.Idle);
    }

    void Update()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic || isExploded || currentState == BarrelState.Knockback) return;
        HandleAIState();
    }

    private void HandleAIState()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, playerDetectionRange, playerLayer);

        if (playerHit != null)
        {
            player = playerHit.transform;
        }

        switch (currentState)
        {
            case BarrelState.Idle:
                rb.linearVelocity = Vector2.zero;

                if (playerHit != null)
                {
                    ChangeState(BarrelState.Awakening);
                }
                break;

            case BarrelState.Awakening:
                rb.linearVelocity = Vector2.zero;

                if (playerHit == null || player == null)
                {
                    ChangeState(BarrelState.Retracting);
                    return;
                }

                float distToPlayerAwake = Vector2.Distance(transform.position, player.position);

                if (distToPlayerAwake <= chaseTriggerRange)
                {
                    ChangeState(BarrelState.Chasing);
                    return;
                }

                awakenTimer += Time.deltaTime;
                if (awakenTimer >= 1.0f)
                {
                    ChangeState(BarrelState.Chasing);
                    return;
                }
                break;

            case BarrelState.Chasing:
                if (playerHit == null || player == null)
                {
                    ChangeState(BarrelState.Retracting);
                    return;
                }

                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer > playerDetectionRange)
                {
                    ChangeState(BarrelState.Retracting);
                    return;
                }

                if (distanceToPlayer <= explodeTriggerRange)
                {
                    ChangeState(BarrelState.Igniting);
                    return;
                }

                MoveTowardsPlayer();
                break;

            case BarrelState.Retracting:
                rb.linearVelocity = Vector2.zero;
                break;

            case BarrelState.Igniting:
                rb.linearVelocity = Vector2.zero;

                if (player == null) return;
                float currentDistance = Vector2.Distance(transform.position, player.position);

                if (currentDistance > explodeTriggerRange)
                {
                    ChangeState(BarrelState.Chasing);
                    return;
                }

                fuseTimer += Time.deltaTime;
                if (fuseTimer >= fuseDuration)
                {
                    PerformExplosion();
                }
                break;

            case BarrelState.Knockback:

                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        if (player.position.x < transform.position.x && facingDirection == -1 ||
            player.position.x > transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void ChangeState(BarrelState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Reset Animator Parameters
        anim.SetBool("isIdle", false);
        anim.SetBool("isChasing", false);
        anim.SetBool("isIgniting", false);

        switch (currentState)
        {
            case BarrelState.Idle:
                anim.SetBool("isIdle", true);
                break;

            case BarrelState.Awakening:
                awakenTimer = 0f;
                break;

            case BarrelState.Chasing:
                anim.SetBool("isChasing", true);
                break;

            case BarrelState.Retracting:
                anim.SetBool("isIdle", true);
                break;

            case BarrelState.Igniting:
                anim.SetBool("isIgniting", true);
                fuseTimer = 0f;
                break;

            case BarrelState.Knockback:
                // anim.SetTrigger("hurt");
                break;
        }
    }
    public void OnAwakenAnimationEnd()
    {
        /*if (currentState == BarrelState.Awakening && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= chaseTriggerRange)
            {
                ChangeState(BarrelState.Chasing);
            }
        }*/
    }

    public void OnRetractAnimationEnd()
    {
        if (currentState == BarrelState.Retracting)
        {
            player = null;
            ChangeState(BarrelState.Idle);
        }
    }

    private void PerformExplosion()
    {
        isExploded = true;
        anim.SetTrigger("explode"); 

        Collider2D[] objectsInExplosion = Physics2D.OverlapCircleAll(transform.position, maxExplosionRadius, playerLayer);

        foreach (Collider2D obj in objectsInExplosion)
        {
            PlayerHealth pHealth = obj.GetComponent<PlayerHealth>();
            if (pHealth != null)
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                float distanceRatio = Mathf.Clamp01(distance / maxExplosionRadius);
                float finalDamage = Mathf.Lerp(maxDamage, minDamage, distanceRatio);

                pHealth.ChangeHealth(-finalDamage);

                PlayerMovement pMovement = obj.GetComponent<PlayerMovement>();
                if (pMovement != null)
                {
                    pMovement.Knockback(transform, knockbackForce, stunTime);
                }
            }
        }

        Destroy(gameObject, 0.9f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseTriggerRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explodeTriggerRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxExplosionRadius);
    }
}