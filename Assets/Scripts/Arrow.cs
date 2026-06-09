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

    public int damage;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

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
        if ((obstacleLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            AttachToTarget(other.gameObject.transform);
            return;
        }

        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (owner == ArrowOwner.Player)
            {
                other.gameObject.GetComponent<Enemy_Health>()?.ChangeHealth(-damage);
                other.gameObject.GetComponent<Enemy_Knockback>()?.Knockback(transform, knockbackForce, knockbackTime, stunTime);
            }
            else if (owner == ArrowOwner.Enemy)
            {
                other.gameObject.GetComponent<PlayerHealth>()?.ChangeHealth(-damage);
                other.gameObject.GetComponent<PlayerMovement>()?.Knockback(transform, knockbackForce, stunTime);
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
