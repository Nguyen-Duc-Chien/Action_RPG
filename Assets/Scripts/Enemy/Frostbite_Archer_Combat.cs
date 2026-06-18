using UnityEngine;

public class Frostbite_Archer_Combat : Archer_Combat
{
    [Header("Frostbite Custom Settings")]
    public float slowDuration = 2.5f;
    [Range(0f, 1f)]
    public float slowAmount = 0.4f;

    public new void Attack()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;
        Transform player = playerObj.transform;

        Vector2 direction = (player.position - firePoint.position).normalized;

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        if (arrow != null)
        {
            arrow.owner = ArrowOwner.Enemy;
            arrow.direction = direction;
            arrow.speed = arrowSpeed;
            arrow.damage = damage; 
            arrow.knockbackForce = knockbackForce;
            arrow.stunTime = stunTime;
            arrow.lifeSpawn = lifeSpawn;
            arrow.targetLayer = targetLayer;

            var slowTrigger = arrowObj.AddComponent<FrostSlowTrigger>();
            slowTrigger.Setup(slowDuration, slowAmount);
        }
    }
}

public class FrostSlowTrigger : MonoBehaviour
{
    private float duration;
    private float amount;

    public void Setup(float slowDuration, float slowAmount)
    {
        duration = slowDuration;
        amount = slowAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_DebuffManager playerDebuff = collision.GetComponent<Player_DebuffManager>();
            if (playerDebuff != null)
            {
                playerDebuff.ApplySlowDebuff(duration, amount);
            }
            Destroy(this);
        }
    }
}