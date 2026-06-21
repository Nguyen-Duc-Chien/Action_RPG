using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator fadeAnim;
    public float fadeTime = 1f;
    public Vector2 newPlayerPosition;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            fadeAnim.Play("FadeToWhite");
            StartCoroutine(DelayFade());
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);

        // Ưu tiên dùng RunManager để về lobby
        if (RunManager.Instance != null)
        {
            RunManager.Instance.GoToLobby();
        }
        else
        {
            // Fallback: dùng sceneToLoad đã set trong Inspector
            player.position = newPlayerPosition;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
