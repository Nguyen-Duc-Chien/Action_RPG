using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    [Header("References")]
    public Transform launchPoint;
    public GameObject arrowPrefab;
    public PlayerMovement playerMovement;
    public Collider2D playerCollider;
    public Animator anim;

    [Header("Bow Settings")]
    public float shootCooldown = 0.5f;
    private float shootTimer;
    public float autoAimRadius = 0.5f; 

    private Vector2 aimDirection = Vector2.right;

    void Update()
    {
        shootTimer -= Time.deltaTime;

        // Update the direction 
        HandleAutoAim();

        if (Input.GetButtonDown("Shoot") && shootTimer <= 0)
        {
            playerMovement.isShooting = true;
            anim.SetBool("isShooting", true);

            if (aimDirection.x < -0.1f)
            {
                playerMovement.ForceFlip(-1);
            }
            else if (aimDirection.x > 0.1f)
            {
                playerMovement.ForceFlip(1);
            }

            //Debug.Log("Shoot button pressed! Auto-aiming target!");
        }
    }

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }

    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    private void HandleAutoAim()
    {
        // 1. Scan all collinders in autoAimRadius that has layer "Enemy"
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, autoAimRadius, enemyLayer);

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        // 2. Find the closest enemy
        foreach (Collider2D enemyCollider in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemyCollider.transform;
            }
        }

        // 3. Calculate the aim direction
        if (closestEnemy != null)
        {
            aimDirection = (closestEnemy.position - launchPoint.position).normalized;
        }
        else
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal != 0 || vertical != 0)
            {
                aimDirection = new Vector2(horizontal, vertical).normalized;
            }
            else
            {
                aimDirection = new Vector2(playerMovement.facingDirection, 0f);
            }
        }

        // After all, update the animator parameters to reflect the aim direction
        anim.SetFloat("aimX", aimDirection.x);
        anim.SetFloat("aimY", aimDirection.y);
    }

    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            GameObject arrowObj = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            if (arrow != null)
            {
                arrow.owner = ArrowOwner.Player;
                arrow.direction = aimDirection; 
                arrow.speed = 12f;

                arrow.damage = StatsManager.Instance.rangeDamage;
                arrow.knockbackForce = StatsManager.Instance.arrowKnockbackForce;
                arrow.knockbackTime = StatsManager.Instance.arrowKnockbackTime;
                arrow.stunTime = StatsManager.Instance.arrowStunTime;

                arrow.targetLayer = LayerMask.GetMask("Enemy");

                Collider2D arrowCollider = arrowObj.GetComponent<Collider2D>();
                if (playerCollider != null && arrowCollider != null)
                {
                    Physics2D.IgnoreCollision(arrowCollider, playerCollider, true);
                }
            }

            shootTimer = shootCooldown;
        }
        anim.SetBool("isShooting", false);
        playerMovement.isShooting = false;
    }

    // Just for debugging: visualize the auto-aim radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }
}

/*
 *  // Outdated code without auto-aiming, kept for reference
 * 
using UnityEngine;

public class Player_Bow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;
    public PlayerMovement playerMovement;

    public Collider2D playerCollider;

    private Vector2 aimDirection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;

    public Animator anim;

    void Update()
    {
        shootTimer -= Time.deltaTime;

        HandleAiming();

        if (Input.GetButtonDown("Shoot") && shootTimer <= 0)
        {
            playerMovement.isShooting = true;
            anim.SetBool("isShooting", true);
            Debug.Log("Shoot button pressed! Shoot animation should play!");
        }
    }

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }

    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    private void HandleAiming()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical !=0)
        {
            aimDirection = new Vector2(horizontal, vertical).normalized;
            anim.SetFloat("aimX", aimDirection.x);
            anim.SetFloat("aimY", aimDirection.y);
        }
    }

    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            if (aimDirection == Vector2.zero || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
            {
                aimDirection = new Vector2(playerMovement.facingDirection, 0f);
            }

            GameObject arrowObj = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            if (arrow != null)
            {
                arrow.owner = ArrowOwner.Player;
                arrow.direction = aimDirection;
                arrow.speed = 12f;
                arrow.damage = StatsManager.Instance.rangeDamage;
                arrow.knockbackForce = StatsManager.Instance.knockbackForce;
                arrow.knockbackTime = StatsManager.Instance.knockbackTime;
                arrow.stunTime = StatsManager.Instance.stunTime;

                arrow.targetLayer = LayerMask.GetMask("Enemy");
                //arrow.obstacleLayer = LayerMask.GetMask("Obstacles");

                Collider2D arrowCollider = arrowObj.GetComponent<Collider2D>();
                if (playerCollider != null && arrowCollider != null)
                {
                    Physics2D.IgnoreCollision(arrowCollider, playerCollider, true);
                }
            }

            shootTimer = shootCooldown;
        }
        anim.SetBool("isShooting", false);
    }
}
*/
