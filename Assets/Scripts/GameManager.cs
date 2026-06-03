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
        if(Instance != null)
        {
            CleanUpAndDestroy();
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
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

        // Destroy persistent objects (if any)
        if (persistentObjects != null)
        {
            foreach (GameObject obj in persistentObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
        }

        // Destroy this GameManager so the scene can create a fresh one if present in the scene
        Destroy(gameObject);

        // Load the configured start scene (fallback to scene index 0 when not set)
        if (!string.IsNullOrEmpty(startSceneName))
            SceneManager.LoadScene(startSceneName);
        else
            SceneManager.LoadScene(0);
    }
}
