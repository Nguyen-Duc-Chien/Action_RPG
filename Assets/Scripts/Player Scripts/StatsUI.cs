using UnityEngine;
using TMPro; // Using this for TextMeshPro

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;

    private bool statsOpen = false;

    public void Start()
    {
        UpdateAllStats();
        statsCanvas.alpha = 0; // Start with stats UI hidden
        statsCanvas.blocksRaycasts = false; // Disable interaction when closed
        statsOpen = false;
    }

    public void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
            if (statsOpen)
            {// Close stats UI
                Time.timeScale = 1;
                UpdateAllStats();
                statsCanvas.alpha = 0;
                statsCanvas.blocksRaycasts = false; // Disable interaction when closed
                statsOpen = false;
                Debug.Log("Stats UI closed!");
            }
            else
            {// Open stats UI
                Time.timeScale = 0;
                UpdateAllStats();
                statsCanvas.alpha = 1;
                statsCanvas.blocksRaycasts = true; // Enable interaction when open
                statsOpen = true;
                Debug.Log("Stats UI opened!");
            }
    }

    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.meleeDamage;
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdateAllStats()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
