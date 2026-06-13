using UnityEngine;

public class Enemy_Ranged_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2.5f;
    [Tooltip("Smaller than 1 so that players can reach easily.")]
    public float retreatSpeedMultiplier = 0.6f;
    public float attackRange = 6f;
    // Distance at which the enemy will start retreating if the player gets too close
    public float retreatRange = 3f;
    public float playerDetectionRange = 9f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Line of Sight Settings")]
    public LayerMask obstacleLayer;

    private float attackCooldownTimer;
    private int facingDirection = -1;
    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Retreating)
            {
                Retreat();
            }
            else if (enemyState == EnemyState.Idle)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // Step closer to the player if they are within detection range but outside of attack range
    void Chase()
    {
        HandleFlip();

        Vector2 direction = (player.position - transform.position).normalized;

        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, direction, 1.0f, obstacleLayer);

        if (wallCheck.collider != null && !wallCheck.collider.CompareTag("Player"))
        {
            Vector2 detourDirection = new Vector2(-direction.y, direction.x);
            rb.linearVelocity = detourDirection * speed;
        }
        else
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void Retreat()
    {
        HandleFlip();
        // Opposite direction to the player
        Vector2 direction = (transform.position - player.position).normalized;

        rb.linearVelocity = direction * (speed * retreatSpeedMultiplier);
    }

    void HandleFlip()
    {
        if (player == null) return;

        if (player.position.x < transform.position.x && facingDirection == -1)
        {
            facingDirection = 1;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (player.position.x > transform.position.x && facingDirection == 1)
        {
            facingDirection = -1;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!CanSeePlayer())
            {
                ChangeState(EnemyState.Chasing);
            }
            else
            {
                float currentRetreatRange = (enemyState == EnemyState.Retreating) ? (retreatRange + 0.5f) : retreatRange;

                if (distanceToPlayer < currentRetreatRange)
                {
                    ChangeState(EnemyState.Retreating);
                }
                else if (distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Idle); 
                }
                else
                {
                    ChangeState(EnemyState.Chasing);
                }
            }

            if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0 && CanSeePlayer())
            {
                attackCooldownTimer = attackCooldown;
                anim.SetTrigger("attackTrigger");
            }

            HandleFlip();
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector2 origin = detectionPoint.position;
        Vector2 direction = ((Vector2)player.position - origin).normalized;
        float distance = Vector2.Distance(origin, player.position);

        bool previousSetting = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);

        Physics2D.queriesStartInColliders = previousSetting;

        Debug.DrawRay(origin, direction * distance, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        SetAnimationBool(enemyState, false);

        enemyState = newState;

        SetAnimationBool(enemyState, true);
    }

    private void SetAnimationBool(EnemyState state, bool value)
    {
        if (anim == null) return;

        switch (state)
        {
            case EnemyState.Idle:
                anim.SetBool("isIdle", value);
                break;
            case EnemyState.Chasing:
            case EnemyState.Retreating:
                anim.SetBool("isChasing", value);
                break;
            case EnemyState.Attacking:
                anim.SetBool("isAttacking", value);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatRange);
    }
}