using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Progress Config")]
    public int targetKillsToWin = 3; 
    public int currentKills;
    private bool isPortalOpened = false;

    [Header("References")]
    [SerializeField] private GameObject nextLevelPortal; 

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
    }

    public void RegisterSpawnedEnemy()
    {
        if (isPortalOpened) return; 

        targetKillsToWin++;
        Debug.Log($"<color=cyan>[LevelManager]</color> Enemy is coming! Target kills to win: {targetKillsToWin}");
    }

    private void HandleMonsterDefeated(int exp)
    {
        currentKills++;
        Debug.Log($"<color=green>[LevelManager]</color> One downed! Process: {currentKills}/{targetKillsToWin}");

        if (currentKills >= targetKillsToWin && !isPortalOpened)
        {
            OpenNextLevelPortal();
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