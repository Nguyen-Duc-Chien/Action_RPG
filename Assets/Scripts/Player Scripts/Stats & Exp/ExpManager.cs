using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f; //Add 20% more exp to level each
    public Slider expSlider;
    public TMP_Text currentLevelText;


    public static event Action<int> OnLevelUp;

    private void Start()
    {
        LoadFromSave();
        UpdateUI();
    }
    public void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Return))
        {
            GainExperience(2); 
        }*/
    }

    private void OnEnable()
    {
        Enemy_Health.OnMonsterDefeated += GainExperience; 
    }
    private void OnDisable()
    {
        Enemy_Health.OnMonsterDefeated -= GainExperience;
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToLevel)
        {
            LevelUp();
        }
        SaveToPlayerPrefs();
        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel; 
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);

        OnLevelUp?.Invoke(level);
    }

    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;
        currentLevelText.text = "Level: " + level;
    }

    #region Save/Load

    public void SaveToPlayerPrefs()
    {
        if (RunManager.Instance == null) return;
        RunManager.Instance.SaveExp(level, currentExp, expToLevel);
    }

    private void LoadFromSave()
    {
        if (RunManager.Instance == null) return;
        if (!RunManager.Instance.HasExpSave()) return;

        RunManager.Instance.LoadExp(out int savedLevel, out int savedExp, out int savedExpToLevel);
        level      = savedLevel;
        currentExp = savedExp;
        expToLevel = savedExpToLevel;
    }

    #endregion
}
