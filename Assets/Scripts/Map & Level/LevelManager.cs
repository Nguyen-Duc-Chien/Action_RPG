using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text levelNameText;

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
        
        if (RunManager.Instance != null)
        {
            var config = RunManager.Instance.GetCurrentConfig();
            if (config != null && config.targetKillsToWin > 0)
            {
                targetKillsToWin = config.targetKillsToWin;
            }
        }
        
        UpdateProgressUI();
    }

    public void RegisterSpawnedEnemy()
    {
        if (isPortalOpened) return;

        if (RunManager.Instance != null)
        {
            var config = RunManager.Instance.GetCurrentConfig();
            if (config != null && config.targetKillsToWin > 0)
            {
                // If we have a fixed target from config, do not increment dynamically
                return;
            }
        }

        targetKillsToWin++;
        UpdateProgressUI();
    }

    public void OverrideKillTarget(int target)
    {
        if (target <= 0) return;
        targetKillsToWin = target;
        UpdateProgressUI();
        Debug.Log($"<color=cyan>[LevelManager]</color> Kill target overridden to: {targetKillsToWin}");
    }

    private void HandleMonsterDefeated(int exp)
    {
        currentKills++;
        UpdateProgressUI();

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
            progressSlider.value    = currentKills;
        }

        if (killCountText != null)
        {
            killCountText.text = $"{currentKills}<size=70%>/{targetKillsToWin}</size>";
        }

        if (levelNameText != null && RunManager.Instance != null)
        {
            var config = RunManager.Instance.GetCurrentConfig();
            if (config != null)
            {
                levelNameText.text = config.levelName;
            }
        }
    }

    private void OpenNextLevelPortal()
    {
        isPortalOpened = true;

        if (RunManager.Instance != null)
        {
            RunManager.Instance.CompleteCurrentLevel();
        }

        if (nextLevelPortal != null)
        {
            nextLevelPortal.SetActive(true);
            Debug.Log("<color=gold>==== Level Clear! ====</color>");
        }
        else
        {
            Debug.LogError("missing nextLevelPortal prefabs!");
        }
    }
}