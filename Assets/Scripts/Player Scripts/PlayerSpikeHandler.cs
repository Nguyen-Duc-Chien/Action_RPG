using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear references from previous scenes and reset player spikes state
        if (isPlayerOnSpikes)
        {
            isPlayerOnSpikes = false;
            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.speed = StatsManager.Instance.speed * 2;
                Debug.Log("[PlayerSpikeHandler] Restored speed on scene load.");
            }
        }
        spikeTilemaps.Clear();
        damageCooldownTimer = 0f;
        Debug.Log($"[PlayerSpikeHandler] Cleared spike tilemaps on loading scene: {scene.name}");
    }

    public void AddSpikeTilemap(Tilemap newSpikeTilemap)
    {
        if (newSpikeTilemap != null && !spikeTilemaps.Contains(newSpikeTilemap))
        {
            spikeTilemaps.Add(newSpikeTilemap);
            Debug.Log($"[PlayerSpikeHandler] Connected spike tilemap from: {newSpikeTilemap.gameObject.transform.parent.parent.name}");
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
            cellPosition.z = 0; // Force Z to 0 to prevent Z-coordinate mismatch issues in 2D tilemaps

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

                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.speed = StatsManager.Instance.speed / 2;
                }

                TakeSpikeDamage();
            }
        }
        else
        {
            if (isPlayerOnSpikes)
            {
                isPlayerOnSpikes = false;
                Debug.Log("Player EXITED spikes - Restoring speed!");

                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.speed = StatsManager.Instance.speed * 2;
                }
            }
        }
    }

    void TakeSpikeDamage()
    {
        if (playerHealth != null)
        {
            playerHealth.ChangeHealth(-damageAmount);
            damageCooldownTimer = damageInterval;
            Debug.Log($"[PlayerSpikeHandler] Dealt {damageAmount} damage to player. Cooldown: {damageInterval}s");
        }
    }
}