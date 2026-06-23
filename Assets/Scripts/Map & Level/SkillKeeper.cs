using UnityEngine;

/// <summary>
/// NPC nằm ở Scene_Starting, cho phép player mở bảng Skill Tree.
/// Cách dùng: Gắn script này vào GameObject NPC, kéo reference vào ToggleSkillTree.
/// Player vào vùng trigger → nhấn Interact → bảng Skill mở.
/// </summary>
public class SkillKeeper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ToggleSkillTree toggleSkillTree;
    [SerializeField] private Animator anim;

    private bool playerInRange = false;

    #region Unity Lifecycle

    private void Update()
    {
        if (!playerInRange) return;

        // Bấm nút tương tác (chữ E / tay cầm)
        if (Input.GetButtonDown("Interact"))
        {
            if (toggleSkillTree != null)
            {
                if (!toggleSkillTree.IsSkillTreeOpen)
                {
                    OpenPanel();
                }
                else
                {
                    ClosePanel();
                }
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            // Bấm nút Hủy (Esc) thì luôn luôn thử đóng
            if (toggleSkillTree != null && toggleSkillTree.IsSkillTreeOpen)
            {
                ClosePanel();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;
        if (anim != null)
            anim.SetBool("playerInRange", true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        if (anim != null) anim.SetBool("playerInRange", false);

        // Đóng panel nếu người chơi đi ra khỏi vùng
        if (toggleSkillTree != null && toggleSkillTree.IsSkillTreeOpen)
        {
            ClosePanel();
        }
    }

    #endregion

    #region Private Helpers

    private void OpenPanel()
    {
        if (toggleSkillTree == null)
        {
            Debug.LogError("[SkillKeeper] ToggleSkillTree reference is missing!");
            return;
        }

        toggleSkillTree.OpenPanel();
        Debug.Log("[SkillKeeper] Skill Panel opened.");
    }

    private void ClosePanel()
    {
        if (toggleSkillTree == null) return;
        
        toggleSkillTree.ClosePanel();
        Debug.Log("[SkillKeeper] Skill Panel closed.");
    }

    #endregion
}
