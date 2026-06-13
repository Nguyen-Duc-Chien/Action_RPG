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
