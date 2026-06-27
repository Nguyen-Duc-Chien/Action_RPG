using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Thanh HP riêng cho Boss, hiển thị trên đầu.
/// Tự tạo Slider trên Editor (World Space Canvas → Slider) rồi kéo vào đây.
/// Slider sẽ giảm mượt mà khi Boss mất máu.
/// </summary>
public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo Slider HP vào đây (tạo sẵn trên Editor)")]
    public Slider hpSlider;

    [Header("Smooth Animation")]
    [Tooltip("Tốc độ slider chạy mượt khi HP thay đổi")]
    public float lerpSpeed = 5f;

    // ===== Runtime =====
    private Enemy_Health healthScript;
    private float displayedHealth;

    void Start()
    {
        healthScript = GetComponent<Enemy_Health>();

        if (healthScript == null)
        {
            Debug.LogWarning("[BossHealthBar] Không tìm thấy Enemy_Health trên " + gameObject.name);
            enabled = false;
            return;
        }

        if (hpSlider == null)
        {
            Debug.LogWarning("[BossHealthBar] Chưa gắn Slider! Kéo Slider vào Inspector.");
            enabled = false;
            return;
        }

        // Khởi tạo
        displayedHealth = healthScript.currentHealth;
        hpSlider.interactable = false;
        hpSlider.maxValue = healthScript.maxHealth;
        hpSlider.value = healthScript.currentHealth;
    }

    void Update()
    {
        if (healthScript == null || hpSlider == null) return;

        float targetHealth = healthScript.currentHealth;

        // Lerp mượt — slider giảm từ từ giống HP bar player
        displayedHealth = Mathf.MoveTowards(displayedHealth, targetHealth, lerpSpeed * healthScript.maxHealth * Time.deltaTime);

        hpSlider.maxValue = healthScript.maxHealth;
        hpSlider.value = displayedHealth;
    }

    void OnDisable()
    {
        // Ẩn thanh HP khi Boss chết
        if (hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false);
        }
    }
}
