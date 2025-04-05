using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static int LastCheckpoint;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetCheckpoint()
    {
        LastCheckpoint = 0;
    }

    private void Start()
    {
        if (LastCheckpoint == GetInstanceID())
        {
            Player.Instance.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            LastCheckpoint = GetInstanceID();
            Debug.Log("New checkpoint");
        }
    }
}