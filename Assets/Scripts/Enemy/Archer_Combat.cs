using UnityEngine;

public class Archer_Combat : MonoBehaviour
{
    [Header("References")]
    public GameObject arrowPrefab;       
    public Transform firePoint;         

    [Header("Arrow Settings For Enemy")]
    public int damage = 4;
    public float arrowSpeed = 8f;
    public float knockbackForce = 6f;
    public float stunTime = 0.2f;
    public float lifeSpawn = 2f;
    public LayerMask targetLayer;        

    private Transform player;

    public virtual void Attack()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("EnemyBowShoot");

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;
        player = playerObj.transform;

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
        }
    }
}