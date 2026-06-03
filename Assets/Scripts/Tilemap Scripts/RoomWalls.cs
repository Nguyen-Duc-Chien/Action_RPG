using UnityEngine;

public class RoomWalls : MonoBehaviour
{
    public GameObject wallUp;
    public GameObject wallDown;
    public GameObject wallLeft;
    public GameObject wallRight;

    public void SetupWalls(bool hasUp, bool hasDown, bool hasLeft, bool hasRight)
    {
        if (wallUp != null) wallUp.SetActive(!hasUp);
        if (wallDown != null) wallDown.SetActive(!hasDown);
        if (wallLeft != null) wallLeft.SetActive(!hasLeft);
        if (wallRight != null) wallRight.SetActive(!hasRight);
    }
}
