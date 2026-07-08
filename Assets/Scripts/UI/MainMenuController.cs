using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM("MenuBGM");

        // Sửa lỗi cảnh báo 2 Audio Listener khi quay về từ màn chơi
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include);
        if (listeners.Length > 1)
        {
            foreach (AudioListener listener in listeners)
            {
                // Nếu listener thuộc về Scene MainMenu (tức là cái mới được tạo ra), ta xóa nó đi 
                // để nhường chỗ cho cái listener bất tử (DontDestroyOnLoad) mang từ màn chơi về.
                if (listener.gameObject.scene.name == "MainMenu")
                {
                    Destroy(listener);
                }
            }
        }
    }

    // Nút Play
    public void PlayGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("UIClick");
        }

        // Nếu đang ở chế độ overlay (MainMenu load additive bên trên scene gameplay)
        // → quay lại game đang pause, không cần load scene mới
        if (StatsUI.isOverlayActive)
        {
            ResumeFromOverlay();
            return;
        }

        // Không có ván dở (mở game lần đầu hoặc sau khi chết) → bắt đầu mới
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM("LobbyBGM");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }
        SceneManager.LoadScene("Scene_Starting");
    }

    /// <summary>
    /// Quay lại game đang pause: unload MainMenu overlay, hiện lại objects, unpause.
    /// </summary>
    private void ResumeFromOverlay()
    {
        StatsUI.isOverlayActive = false;

        // Hiện lại persistent objects (Player, HUD, Canvas)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetPersistentObjectsVisible(true);
        }

        // Phát lại BGM gameplay phù hợp với scene đang chơi
        if (AudioManager.Instance != null && RunManager.Instance != null)
        {
            string activeScene = GetActiveGameplaySceneName();
            if (activeScene == RunManager.Instance.lobbySceneName)
                AudioManager.Instance.PlayBGM("LobbyBGM");
            else if (activeScene == RunManager.Instance.forestSceneName)
                AudioManager.Instance.PlayBGM("ForestBGM");
            else if (activeScene == RunManager.Instance.dungeonSceneName)
                AudioManager.Instance.PlayBGM("DungeonBGM");
            else
                AudioManager.Instance.PlayBGM("LobbyBGM");
        }

        // Unpause game
        Time.timeScale = 1f;

        // Unload MainMenu scene overlay
        SceneManager.UnloadSceneAsync("MainMenu");

        Debug.Log("[MainMenuController] Resumed from overlay. Game unpaused.");
    }

    /// <summary>
    /// Tìm tên scene gameplay đang active (không phải MainMenu).
    /// </summary>
    private string GetActiveGameplaySceneName()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "MainMenu" && scene.isLoaded)
                return scene.name;
        }
        return "";
    }

    // Nút Exit
    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("UIClick");
        Debug.Log("Game Exiting...");
        Application.Quit();
    }
}

