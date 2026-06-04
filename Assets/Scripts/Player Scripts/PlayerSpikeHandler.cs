using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PlayerSpikeHandler : MonoBehaviour
{
    [Header("References")]
    public List<Tilemap> spikeTilemaps = new List<Tilemap>();
    private PlayerHealth playerHealth;

    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float damageInterval = 1f;

    private float damageCooldownTimer;
    private bool isPlayerOnSpikes = false;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void AddSpikeTilemap(Tilemap newSpikeTilemap)
    {
        if (newSpikeTilemap != null && !spikeTilemaps.Contains(newSpikeTilemap))
        {
            spikeTilemaps.Add(newSpikeTilemap);
            Debug.Log($"[Spike Handler] Successfully connected Spikes from: {newSpikeTilemap.gameObject.transform.parent.parent.name}");
        }
    }

    void Update()
    {
        CheckIfStandingOnSpikes();

        if (isPlayerOnSpikes)
        {
            damageCooldownTimer -= Time.deltaTime;
            if (damageCooldownTimer <= 0)
            {
                TakeSpikeDamage();
            }
        }
    }

    void CheckIfStandingOnSpikes()
    {
        if (spikeTilemaps.Count == 0) return;

        bool currentlyOnSpike = false;

        foreach (Tilemap tilemap in spikeTilemaps)
        {
            if (tilemap == null) continue;

            Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

            if (tilemap.HasTile(cellPosition))
            {
                currentlyOnSpike = true;
                break;
            }
        }

        if (currentlyOnSpike)
        {
            if (!isPlayerOnSpikes)
            {
                isPlayerOnSpikes = true;
                Debug.Log("Player ENTERED spikes - Slowing down!");

                StatsManager.Instance.speed = 2.5f;

                TakeSpikeDamage();
            }
        }
        else
        {
            if (isPlayerOnSpikes)
            {
                isPlayerOnSpikes = false;
                Debug.Log("Player EXITED spikes - Restoring speed!");

                StatsManager.Instance.speed = 5f;
            }
        }
    }

    void TakeSpikeDamage()
    {
        if (playerHealth != null)
        {
            playerHealth.ChangeHealth(-damageAmount);
            damageCooldownTimer = damageInterval;
        }
    }
}