using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        // Có thể thực hiện các khởi tạo cần thiết ở đây nếu có
    }

    // Nút Play
    public void PlayGame()
    {
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
        Debug.Log("Game Exiting...");
        Application.Quit();
    }
}
