using UnityEngine;
using System.Collections.Generic;

public class Archer_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3.5f;
    [Tooltip("Smaller than 1 so that players can reach easily.")]
    public float retreatSpeedMultiplier = 0.6f;
    public float attackRange = 5f;
    // Distance at which the enemy will start retreating if the player gets too close
    public float retreatRange = 3f;
    public float playerDetectionRange = 6f;
    public float attackCooldown = 1.5f;

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
    private AStarPathfinder pathfinder;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Setup A* pathfinder
        pathfinder = GetComponent<AStarPathfinder>();
        if (pathfinder == null)
        {
            pathfinder = gameObject.AddComponent<AStarPathfinder>();
        }
        pathfinder.obstacleLayer = obstacleLayer;

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
    // Uses A* pathfinding to navigate around obstacles
    void Chase()
    {
        HandleFlip();

        Vector2 direction = pathfinder.GetDirectionToTarget(player.position);

        if (direction != Vector2.zero)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // If A* can't find a path (e.g. player is completely blocked off), stop moving to avoid stuttering into walls
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Retreat()
    {
        HandleFlip();

        // Calculate a retreat point away from the player, then pathfind to it
        Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;
        Vector2 retreatTarget = (Vector2)transform.position + awayFromPlayer * retreatRange;

        Vector2 direction = pathfinder.GetDirectionToTarget(retreatTarget);

        if (direction != Vector2.zero)
        {
            rb.linearVelocity = direction * (speed * retreatSpeedMultiplier);
        }
        else
        {
            // If A* can't find a path (e.g. backed into a corner), stop moving instead of pushing blindly into the wall
            rb.linearVelocity = Vector2.zero;
        }
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
            bool canSee = CanSeePlayer();
            bool clearShot = canSee && HasClearShot();

            if (!canSee)
            {
                // Can't see player at all -> chase around obstacles
                ChangeState(EnemyState.Chasing);
            }
            else if (!clearShot)
            {
                // Can see player but no clear shot (obstacle partially blocking) -> keep moving
                ChangeState(EnemyState.Chasing);
            }
            else
            {
                // Full clear line of fire
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

            // Only attack when we have a clear, unobstructed shot
            if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0 && clearShot)
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

    /// <summary>
    /// Checks if the archer has a WIDE clear shot to the player.
    /// Unlike CanSeePlayer (thin raycast), this uses a box/circle cast
    /// to make sure the arrow won't clip an obstacle edge.
    /// </summary>
    private bool HasClearShot()
    {
        if (player == null) return false;

        Vector2 origin = detectionPoint.position;
        Vector2 target = player.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);

        bool previousSetting = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        // Use CircleCast with a small radius to simulate arrow width
        // This catches cases where a thin raycast passes through but an arrow would hit the wall
        RaycastHit2D hit = Physics2D.CircleCast(origin, 0.25f, direction, distance, obstacleLayer);

        Physics2D.queriesStartInColliders = previousSetting;

        // Draw debug line (blue = clear shot, yellow = blocked)
        Debug.DrawRay(origin, direction * distance, hit.collider == null ? Color.blue : Color.yellow);

        return hit.collider == null;
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