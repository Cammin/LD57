using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool IsStart;
    
    private static int LastCheckpoint;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetCheckpoint()
    {
        LastCheckpoint = 0;
    }

    private void Start()
    {
        if (IsStart && LastCheckpoint == 0)
        {
            Player.Instance.transform.position = transform.position;
            return;
        }
        
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