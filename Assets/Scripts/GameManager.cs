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
        SceneManager.sceneLoaded += OnSceneLoadedForUI;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedForUI;
    }

    private void OnSceneLoadedForUI(Scene scene, LoadSceneMode mode)
    {
        // Bỏ qua khi MainMenu được load additive (overlay mode)
        if (mode == LoadSceneMode.Additive) return;

        // ── Xóa duplicate EventSystem (giữ lại cái persistent, xóa cái scene-local) ──
        // Khi scene dungeon/forest có EventSystem riêng + persistent EventSystem → UI chết
        UnityEngine.EventSystems.EventSystem[] allES = 
            FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsInactive.Include);
        if (allES.Length > 1)
        {
            foreach (var es in allES)
            {
                // Xóa EventSystem nằm trong scene vừa load (không phải DontDestroyOnLoad)
                if (es.gameObject.scene == scene)
                {
                    Debug.Log($"<color=yellow>[GameManager]</color> Removing duplicate EventSystem from scene: {scene.name}");
                    Destroy(es.gameObject);
                }
            }
        }

        bool inMainMenu = scene.name == "MainMenu";
        foreach (GameObject obj in persistentObjects)
        {
            if (obj == null) continue;
            
            // Nếu là Camera hoặc Player thì để nguyên không ẩn (để tránh lỗi freeze hoặc mất cam)
            if (obj.name.Contains("Camera") || obj.CompareTag("Player"))
            {
                continue;
            }

            // Nếu là các thành phần giao diện (UI, Canvas) thì ẩn khi ở MainMenu
            if (obj.name.Contains("Canvas") || obj.name.Contains("UI"))
            {
                obj.SetActive(!inMainMenu);
            }
        }
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

    /// <summary>
    /// Ẩn/hiện các persistent objects khi chuyển sang Main Menu overlay.
    /// </summary>
    public void SetPersistentObjectsVisible(bool visible)
    {
        foreach (GameObject obj in persistentObjects)
        {
            if (obj == null) continue;
            obj.SetActive(visible);
        }
    }

    private void CleanUpAndDestroy()
    {
        // Destroy the current instance and all its persistent objects
        foreach (GameObject obj in persistentObjects)
        {            
            if (obj != null) Destroy(obj);
        }
        Destroy(gameObject);
    }

    public void DestroyPersistentState()
    {
        CleanUpAndDestroy();
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
