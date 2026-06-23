using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent Objects")]
    public GameObject[] persistentObjects;

    [Header("Cached References")]
    public Camera shopCamera;
    public CanvasGroup canvasGroup;
    public ShopManager shopManager;

    [Header("Runtime")]
    [Tooltip("Scene name to load when starting a new run / restarting after death.")]
    public string startSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            CleanUpAndDestroy();
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        MarkPersistentObjects();
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        // Destroy the current instance and all its persistent objects
        foreach (GameObject obj in persistentObjects)
        {            
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Restart the run: restore timeScale, remove persistent objects and reload the configured start scene.
    /// </summary>
    public void RestartRun()
    {
        Time.timeScale = 1f;

        if (RunManager.Instance != null)
        {
            RunManager.Instance.ResetAllProgress();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!string.IsNullOrEmpty(startSceneName))
            SceneManager.LoadScene(startSceneName);
        else
            SceneManager.LoadScene(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            Vector3 targetSpawnPos = Vector3.zero;
            GameObject spawnPointObj = GameObject.Find("SpawnPoint");

            if (spawnPointObj != null)
            {
                targetSpawnPos = spawnPointObj.transform.position;
                Debug.Log("Found spawnPoint at: " + targetSpawnPos);
            }
            else
            {
                Debug.LogWarning("There isn't GameObject called 'SpawnPoint' in this Scene! Spawn at (0,0,0)");
            }

            playerHealth.ResetPlayer(targetSpawnPos);
        }
        else
        {
            Debug.LogWarning("Not found PlayerHealth to Reset!");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
