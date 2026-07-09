using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevation_Entry : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayer = collision.gameObject.CompareTag("Player");
        bool isNonAStarEnemy = collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<AStarPathfinder>() == null;

        if (isPlayer)
        {
            // Player: toggle global colliders as original
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = false;
            }

            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = true;
            }

            SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 12;
            }
        }
        else if (isNonAStarEnemy)
        {
            // Enemy: per-entity ignore collision, không tắt collider toàn cục
            foreach (Collider2D mountain in mountainColliders)
            {
                if (mountain != null)
                    Physics2D.IgnoreCollision(collision, mountain, true);
            }

            SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 12;
            }
        }
    }
}
