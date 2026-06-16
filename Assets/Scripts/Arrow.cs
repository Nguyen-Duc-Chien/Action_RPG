using UnityEngine;

public enum ArrowOwner { Player, Enemy }

public class Arrow : MonoBehaviour
{
    public ArrowOwner owner;

    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;
    public float lifeSpawn = 2;
    public float speed;

    public LayerMask targetLayer;
    public LayerMask obstacleLayer;

    public SpriteRenderer sr;
    public Sprite buriedSprite;

    public float damage;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;
    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = direction * speed;
        RotateArrow();
        Destroy(gameObject, lifeSpawn);
    }

    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == ArrowOwner.Player && other.CompareTag("Player")) return;
        if (owner == ArrowOwner.Enemy && other.CompareTag("Enemy")) return;

        if ((obstacleLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            AttachToTarget(other.gameObject.transform);
            return;
        }

        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (owner == ArrowOwner.Enemy)
            {
                other.gameObject.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
                other.gameObject.GetComponent<PlayerMovement>()?.Knockback(transform, knockbackForce, stunTime);
            }
            else if (owner == ArrowOwner.Player)
            {
                Enemy_Health enemyHealth = other.gameObject.GetComponent<Enemy_Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.ChangeHealth(-damage);
                }
                Enemy_Knockback enemyKnockback = other.gameObject.GetComponent<Enemy_Knockback>();
                if (enemyKnockback != null)
                {
                    enemyKnockback.Knockback(transform, knockbackForce, knockbackTime, stunTime);
                }
                Enemy_DebuffManager debuffManager = other.gameObject.GetComponent<Enemy_DebuffManager>();
                if (debuffManager != null)
                {
                    debuffManager.ApplyArrowDebuff();
                }
            }
            AttachToTarget(other.gameObject.transform);
        }
    }

    private void AttachToTarget(Transform target)
    {
        Collider2D arrowCollider = GetComponent<Collider2D>();
        if (arrowCollider != null)
        {
            arrowCollider.enabled = false;
        }

        sr.sprite = buriedSprite;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        transform.SetParent(target);

        Destroy(gameObject, 3f);
    }

}
