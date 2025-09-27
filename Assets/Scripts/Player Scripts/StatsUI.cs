using UnityEngine;
using TMPro; // Using this for TextMeshPro

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;

    private bool statsOpen = false;

    public void Start()
    {
        UpdateAllStart();
        statsCanvas.alpha = 0; // Start with stats UI hidden
        statsOpen = false;
    }

    public void Update()
    {
        if (Input.GetButtonDown("ToggleStats"))
            if (statsOpen)
            {// Close stats UI
                Time.timeScale = 1;
                UpdateAllStart();
                statsCanvas.alpha = 0;
                statsOpen = false;
            }
            else
            {// Open stats UI
                Time.timeScale = 0;
                UpdateAllStart();
                statsCanvas.alpha = 1;
                statsOpen = true;
            }
    }

    public void UpdateDamage()
    {
        statsSlots[0].GetComponentInChildren<TMP_Text>().text = "Damage: " + StatsManager.Instance.damage;
    }

    public void UpdateSpeed()
    {
        statsSlots[1].GetComponentInChildren<TMP_Text>().text = "Speed: " + StatsManager.Instance.speed;
    }

    public void UpdateAllStart()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
