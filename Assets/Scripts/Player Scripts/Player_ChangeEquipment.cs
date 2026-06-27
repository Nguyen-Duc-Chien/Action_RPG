using UnityEngine;

public class Player_ChangeEquipment : MonoBehaviour
{
    public Player_Combat combat;
    public Player_Bow bow;

    private void Start()
    {
        if (combat != null) combat.enabled = true;
        if (bow != null) bow.enabled = true;

        if (combat != null && combat.anim != null)
        {
            combat.anim.SetLayerWeight(0, 1);
            combat.anim.SetLayerWeight(1, 0);
        }
    }

    private void Update()
    {
        // Disabled swapping since both weapons use left and right click independently now.
    }
}