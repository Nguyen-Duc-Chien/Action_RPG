using UnityEngine;

public class RockBoss_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float playerDetectionRange = 10f;
    public float stopDistance = 2f;

    [Header("References")]
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private AStarPathfinder pathfinder;
    
    private EnemyState enemyState; 
    private float idleWaitTimer = 0f; 
    private float roarTimer = 0f;

    private float lastMoveX = 0f;
    private float lastMoveY = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pathfinder = GetComponent<AStarPathfinder>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (idleWaitTimer > 0) idleWaitTimer -= Time.deltaTime;
        if (roarTimer > 0) roarTimer -= Time.deltaTime;

        if (enemyState != EnemyState.Knockback && enemyState != EnemyState.Attacking)
        {
            if (idleWaitTimer <= 0)
            {
                CheckForPlayer();
            }
            
            if (enemyState == EnemyState.Chasing)
            {
                ChasePlayer();
            }
            else
            {
                // Idle
                rb.linearVelocity = Vector2.zero;
                UpdateAnimator(Vector2.zero);
            }
        }
        else if (enemyState == EnemyState.Attacking)
        {
            // Đứng im khi đang tấn công
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);
        if (hits.Length > 0)
        {
            if (roarTimer <= 0)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("BossRoar");
                roarTimer = 6f; // Roar every 6 seconds when player is near
            }

            player = hits[0].transform;
            float distance = Vector2.Distance(transform.position, player.position);
            
            // Nếu người chơi ở xa thì mới chạy đuổi theo
            if (distance > stopDistance)
            {
                ChangeState(EnemyState.Chasing);
            }
            else
            {
                // Nếu đã áp sát rồi thì đứng im (Idle) chờ hồi chiêu để đánh
                ChangeState(EnemyState.Idle);
            }
        }
        else
        {
            player = null;
            ChangeState(EnemyState.Idle);
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction;
        
        // Sử dụng A* nếu có gắn component, nếu không thì dùng đường chim bay
        if (pathfinder != null)
        {
            direction = pathfinder.GetDirectionToTarget(player.position);
        }
        else
        {
            direction = (player.position - transform.position).normalized;
        }

        rb.linearVelocity = direction * speed;

        // Truyền Parameter vào Animator
        UpdateAnimator(direction);
    }

    private void UpdateAnimator(Vector2 direction)
    {
        // Truyền hướng di chuyển hiện tại
        anim.SetFloat("moveX", direction.x);
        anim.SetFloat("moveY", direction.y);

        // Chỉ cập nhật LastMove khi Boss thực sự có di chuyển (magnitude > 0.1)
        if (direction.magnitude > 0.1f)
        {
            lastMoveX = direction.x;
            lastMoveY = direction.y;
        }

        // Truyền hướng cuối cùng để dùng cho Blend Tree Idle và Attack
        anim.SetFloat("lastMoveX", lastMoveX);
        anim.SetFloat("lastMoveY", lastMoveY);
    }

    public void ChangeState(EnemyState newState)
    {
        // Nếu state không đổi thì bỏ qua
        if (enemyState == newState) return;

        // Tắt animation cũ
        if (enemyState == EnemyState.Idle) anim.SetBool("isIdle", false);
        else if (enemyState == EnemyState.Chasing) anim.SetBool("isChasing", false);

        enemyState = newState;

        // Bật animation mới
        if (enemyState == EnemyState.Idle) anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing) anim.SetBool("isChasing", true);
    }

    public void FinishAttack()
    {
        ChangeState(EnemyState.Idle);
        // Delay 0.1s để Animator chắc chắn kịp cập nhật trạng thái Idle trước khi Chase tiếp
        idleWaitTimer = 0.1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
    }

    // Helper getter cho script Combat
    public EnemyState GetCurrentState()
    {
        return enemyState;
    }

    public Transform GetPlayerTransform()
    {
        return player;
    }
}
