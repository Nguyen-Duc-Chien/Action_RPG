using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;

    private void Update()
    {
        if (Input.GetButtonDown("ChangeEquipment"))
        {
            if (combat.enabled)
            {
                combat.enabled = false;
                bow.enabled = true;
            }
            else
            {
                combat.enabled = true;
                bow.enabled = false;
            }
        }
    }
}
