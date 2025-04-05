using UnityEngine;

public class ClockOutStation : MonoBehaviour
{
    public bool PlayerInRange;
    public ExitDoor Door;

    private void Update()
    {
        if (PlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            ClockOut();
        }
    }

    private void ClockOut()
    {
        Door.OpenDoor();
    }
}