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


    #endregion

    #region Private State

    private enum Tab { Forest, Dungeon }
    private Tab currentTab = Tab.Forest;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (panelCanvasGroup == null)
            panelCanvasGroup = GetComponent<CanvasGroup>();
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



            TMP_Text label = btnObj.transform.Find("Label")?.GetComponent<TMP_Text>();
            if (label != null)
            {
                int    displayNum = (currentTab == Tab.Forest) ? levelNum : levelNum - 10;

                string numStr   = $"<b>{displayNum}</b>";
                string doneStr  = completed ? " <color=#7EC87A><size=70%>Completed</size></color>" : "";

                label.text = $"{numStr} {doneStr}";
            }

            CanvasGroup cg = btnObj.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                if (!unlocked)       cg.alpha = 0.35f; // locked
                else if (completed)  cg.alpha = 1.00f; // completed — view only
                else                 cg.alpha = 1.00f; // current — playable
            }

            // Chỉ cho chơi level hiện tại (unlocked + chưa completed)
            // Nếu đang debug → tất cả level đều chơi được
            bool debugMode = RunManager.Instance.unlockAllLevels;
            bool playable  = debugMode ? true : (unlocked && !completed);

            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = playable;
                
                if (completed && !playable)
                {
                    ColorBlock cb = btn.colors;
                    cb.disabledColor = cb.normalColor;
                    btn.colors = cb;
                }

                btn.onClick.RemoveAllListeners();
                if (playable)
                {
                    btn.onClick.AddListener(() =>
                    {
                        Hide();
                        Time.timeScale = 1f;
                        RunManager.Instance.PlayLevel(levelNum - 1);
                    });
                }
            }
        }
    }

    #endregion

    #region Helpers

    private void SetCanvasVisible(bool visible)
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.alpha          = visible ? 1f : 0f;
            panelCanvasGroup.blocksRaycasts = visible;
            panelCanvasGroup.interactable   = visible;
        }
    }

    #endregion
}
