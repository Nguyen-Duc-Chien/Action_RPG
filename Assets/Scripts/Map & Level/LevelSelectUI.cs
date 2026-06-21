using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectUI : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private LevelSelectKeeper keeper;

    [Header("Tab Buttons")]
    [SerializeField] private Button btnForest;
    [SerializeField] private Button btnDungeon;

    [SerializeField] private Color tabActiveColor   = new Color(0.95f, 0.88f, 0.65f);
    [SerializeField] private Color tabInactiveColor = new Color(0.20f, 0.18f, 0.22f);

    [Header("Level Button Colors")]
    [SerializeField] private Color colorForest  = new Color(0.10f, 0.28f, 0.12f);
    [SerializeField] private Color colorDungeon = new Color(0.18f, 0.08f, 0.30f);
    [SerializeField] private Color colorLocked  = new Color(0.12f, 0.11f, 0.13f);
    [SerializeField] private Color colorBoss    = new Color(0.35f, 0.06f, 0.06f);

    [Header("Level Button Text Colors")]
    [SerializeField] private Color textUnlocked = new Color(0.95f, 0.92f, 0.85f);
    [SerializeField] private Color textLocked   = new Color(0.45f, 0.43f, 0.48f);

    #endregion

    #region Private State

    private enum Tab { Forest, Dungeon }
    private Tab currentTab = Tab.Forest;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        SetCanvasVisible(false);
    }

    private void Start()
    {
        if (btnForest  != null) btnForest.onClick.AddListener(() => SwitchTab(Tab.Forest));
        if (btnDungeon != null) btnDungeon.onClick.AddListener(() => SwitchTab(Tab.Dungeon));
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    #endregion

    #region Public API

    public void Show()
    {
        SetCanvasVisible(true);
        RefreshUI();
    }

    public void Hide()
    {
        SetCanvasVisible(false);
    }

    public void OnCloseBtnClicked()
    {
        if (keeper != null) keeper.ClosePanel();
        else Hide();
    }

    #endregion

    #region Tab Logic

    private void SwitchTab(Tab tab)
    {
        currentTab = tab;
        UpdateTabVisuals();
        RefreshUI();
    }

    private void UpdateTabVisuals()
    {
        if (btnForest != null)
        {
            bool active = currentTab == Tab.Forest;
            btnForest.image.color = active ? tabActiveColor : tabInactiveColor;
            TMP_Text t = btnForest.GetComponentInChildren<TMP_Text>();
            if (t != null) t.color = active ? new Color(0.10f, 0.08f, 0.05f) : new Color(0.6f, 0.58f, 0.62f);
        }
        if (btnDungeon != null)
        {
            bool active = currentTab == Tab.Dungeon;
            btnDungeon.image.color = active ? tabActiveColor : tabInactiveColor;
            TMP_Text t = btnDungeon.GetComponentInChildren<TMP_Text>();
            if (t != null) t.color = active ? new Color(0.10f, 0.08f, 0.05f) : new Color(0.6f, 0.58f, 0.62f);
        }
    }

    #endregion

    #region UI Generation

    private void RefreshUI()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        if (RunManager.Instance == null)
        {
            Debug.LogWarning("[LevelSelectUI] RunManager.Instance is null!");
            return;
        }

        UpdateTabVisuals();

        int startLevel = (currentTab == Tab.Forest) ? 1  : 11;
        int endLevel   = (currentTab == Tab.Forest) ? 10 : 20;

        for (int i = startLevel; i <= endLevel; i++)
        {
            int levelNum = i;

            bool unlocked  = RunManager.Instance.IsLevelUnlocked(levelNum);
            bool completed = RunManager.Instance.IsLevelCompleted(levelNum);

            LevelConfig cfg = RunManager.Instance.GetConfig(levelNum - 1);
            bool isBoss = (cfg != null && cfg.hasBoss);

            // ── Tạo nút ──────────────────────────────────────────────────────
            GameObject btnObj = Instantiate(levelButtonPrefab, buttonContainer);
            btnObj.name = "LevelBtn_" + levelNum;

            // ── Màu nền ───────────────────────────────────────────────────────
            Image bg = btnObj.GetComponent<Image>();
            if (bg != null)
            {
                if (!unlocked)                     bg.color = colorLocked;
                else if (isBoss)                   bg.color = colorBoss;
                else if (currentTab == Tab.Forest) bg.color = colorForest;
                else                               bg.color = colorDungeon;
            }

            TMP_Text label = btnObj.transform.Find("Label")?.GetComponent<TMP_Text>();
            if (label != null)
            {
                label.color = unlocked ? textUnlocked : textLocked;

                int    displayNum = (currentTab == Tab.Forest) ? levelNum : levelNum - 10;
                string biomeHex   = (currentTab == Tab.Forest) ? "#7EC87A" : "#B07EE8";
                string biomeIcon  = (currentTab == Tab.Forest) ? "🌲" : "🏰";

                string biomeStr = $"<color={biomeHex}><size=80%>{biomeIcon}</size></color>";
                string numStr   = $"<b>{displayNum}</b>";
                string doneStr  = completed ? " <color=#7EC87A><size=70%>✓</size></color>" : "";
                string bossStr  = isBoss ? "\n<color=#FF6B6B><size=65%>💀 BOSS</size></color>" : "";

                label.text = $"{biomeStr} {numStr}{doneStr}{bossStr}";
            }

            TMP_Text subLabel = btnObj.transform.Find("SubLabel")?.GetComponent<TMP_Text>();
            if (subLabel != null)
            {
                if (unlocked && cfg != null && cfg.targetKillsToWin > 0)
                {
                    string killColor = isBoss ? "#FF9999" : "#AAAAAA";
                    subLabel.text = $"<color={killColor}><size=85%>⚔ {cfg.targetKillsToWin} kills</size></color>";
                }
                else
                {
                    subLabel.text = unlocked ? "" : "<color=#444444><size=80%>🔒</size></color>";
                }
            }

            Transform completedIcon = btnObj.transform.Find("CompletedIcon");
            if (completedIcon != null) completedIcon.gameObject.SetActive(completed);

            Transform lockedIcon = btnObj.transform.Find("LockedIcon");
            if (lockedIcon != null) lockedIcon.gameObject.SetActive(!unlocked);

            CanvasGroup cg = btnObj.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = unlocked ? 1f : 0.35f;

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = unlocked;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Hide();
                    Time.timeScale = 1f;
                    RunManager.Instance.PlayLevel(levelNum - 1);
                });
            }
        }
    }

    #endregion

    #region Helpers

    private void SetCanvasVisible(bool visible)
    {
        if (panelCanvasGroup == null) return;
        panelCanvasGroup.alpha          = visible ? 1f : 0f;
        panelCanvasGroup.blocksRaycasts = visible;
        panelCanvasGroup.interactable   = visible;
    }

    #endregion
}
