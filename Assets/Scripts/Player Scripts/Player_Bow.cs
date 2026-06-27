using UnityEngine;
using UnityEngine.EventSystems;

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

        if (Input.GetMouseButtonDown(1) && shootTimer <= 0)
        {
            if (StatsManager.Instance != null && !StatsManager.Instance.isBowUnlocked) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            anim.SetLayerWeight(0, 0);
            anim.SetLayerWeight(1, 1);

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

            //Debug.Log("Right Mouse pressed! Auto-aiming target!");
        }
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
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("PlayerBow");
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