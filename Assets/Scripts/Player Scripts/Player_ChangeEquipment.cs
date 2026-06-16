using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;

    private void Start()
    {
        if (StatsManager.Instance == null || !StatsManager.Instance.isBowUnlocked)
        {
            if (combat != null) combat.enabled = true;
            if (bow != null) bow.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            if (StatsManager.Instance == null || !StatsManager.Instance.isBowUnlocked)
            {
                if (bow != null && bow.enabled) bow.enabled = false;
                if (combat != null && !combat.enabled) combat.enabled = true;
                return;
            }
            if (combat.enabled)
            {
                combat.enabled = false;
                bow.enabled = true;
                Debug.Log("Switched to Bow!");
            }
            else
            {
                combat.enabled = true;
                bow.enabled = false;
                Debug.Log("Switched to Melee!");
            }
        }
    }
}