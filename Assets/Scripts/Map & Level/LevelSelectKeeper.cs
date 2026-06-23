using UnityEngine;

/// <summary>
/// NPC nằm ở Scene_Starting, cho phép player mở bảng chọn màn.
/// Cách dùng: Gắn script này vào GameObject NPC, kéo reference vào LevelSelectUI.
/// Player vào vùng trigger → nhấn Interact → panel mở (game pause).
/// Nhấn Cancel hoặc nút X trên panel → đóng.
/// </summary>
public class LevelSelectKeeper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelSelectUI levelSelectUI;
    [SerializeField] private Animator anim;

    private bool playerInRange = false;
    private bool isPanelOpen   = false;

    #region Unity Lifecycle

    private void Update()
    {
        if (!playerInRange) return;

        if (!isPanelOpen && Input.GetButtonDown("Interact"))
        {
            OpenPanel();
        }
        else if (isPanelOpen && Input.GetButtonDown("Cancel"))
        {
            ClosePanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;
        //Debug.Log($"[LevelSelectKeeper] Player entered range. anim is null? {anim == null}");
        if (anim != null)
            anim.SetBool("playerInRange", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        if (anim != null) anim.SetBool("playerInRange", false);

        if (isPanelOpen) ClosePanel();
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════════
    #region Public API

    public void ClosePanel()
    {
        if (!isPanelOpen) return;

        isPanelOpen = false;
        Time.timeScale = 1f;

        if (levelSelectUI != null) levelSelectUI.Hide();

        Debug.Log("[LevelSelectKeeper] Panel closed.");
    }

    #endregion

    // ══════════════════════════════════════════════════════════════════════════
    #region Private

    private void OpenPanel()
    {
        if (levelSelectUI == null)
        {
            Debug.LogError("[LevelSelectKeeper] LevelSelectUI reference is missing!");
            return;
        }

        isPanelOpen = true;
        Time.timeScale = 0f;

        levelSelectUI.Show();
        Debug.Log("[LevelSelectKeeper] Panel opened.");
    }
    

    #endregion
}
