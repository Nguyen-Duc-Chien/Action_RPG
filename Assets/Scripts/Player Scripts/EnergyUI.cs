using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyUI : MonoBehaviour
{
    public static EnergyUI Instance { get; private set; }

    [Header("UI Components")]
    public Slider energySlider;
    public TMP_Text energyText;

    public EnergyManager energyManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (energySlider != null)
        {
            energySlider.interactable = false;
        }
    }

    private void OnEnable()
    {
        if (energyManager != null)
        {
            energyManager.OnEnergyChanged += UpdateEnergyUI;
        }
    }

    private void OnDisable()
    {
        if (energyManager != null)
        {
            energyManager.OnEnergyChanged -= UpdateEnergyUI;
        }
    }

    public void UpdateEnergyUI(float currentEnergy, float maxEnergy)
    {
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }

        if (energyText != null)
        {
            energyText.text = "Energy : " + Mathf.RoundToInt(currentEnergy) + " / " + Mathf.RoundToInt(maxEnergy);
        }
    }
}
