using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Progress Config")]
    public int targetKillsToWin = 0; 
    public int currentKills;
    private bool isPortalOpened = false;

    [Header("References")]
    [SerializeField] private GameObject nextLevelPortal;
    [SerializeField] private Slider progressSlider;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += HandleMonsterDefeated;
    }

    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= HandleMonsterDefeated;
    }

    private void Start()
    {
        if (nextLevelPortal != null) nextLevelPortal.SetActive(false);
        UpdateProgressUI();     // Initialize UI at the start
    }

    public void RegisterSpawnedEnemy()
    {
        if (isPortalOpened) return; 

        targetKillsToWin++;

        UpdateProgressUI();     // Update Max Value when new enemy is registered
    }

    private void HandleMonsterDefeated(int exp)
    {
        currentKills++;
        //Debug.Log($"<color=green>[LevelManager]</color> One downed! Process: {currentKills}/{targetKillsToWin}");

        UpdateProgressUI();     // Update current value when an enemy is defeated

        if (currentKills >= targetKillsToWin && !isPortalOpened)
        {
            OpenNextLevelPortal();
        }
    }

    private void UpdateProgressUI()
    {
        if (progressSlider != null)
        {
            progressSlider.maxValue = targetKillsToWin;
            progressSlider.value = currentKills;
        }
    }

    private void OpenNextLevelPortal()
    {
        isPortalOpened = true;
        if (nextLevelPortal != null)
        {
            nextLevelPortal.SetActive(true);
            Debug.Log("<color=gold>==== The new portal is opened! ==== Come to spawn for next level.</color>");
        }
        else
        {
            Debug.LogError("LevelManager is missing Prefab Next Level Portal!");
        }
    }
}