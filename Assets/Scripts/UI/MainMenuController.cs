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
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlaySFX("UIClick");
            AudioManager.Instance.PlayBGM("LobbyBGM");
        }
        // Chuyển thẳng vào scene Starting như yêu cầu
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }
        SceneManager.LoadScene("Scene_Starting");
    }

    // Nút Exit
    public void ExitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("UIClick");
        Debug.Log("Game Exiting...");
        Application.Quit();
    }
}
