using LDtkUnity;
using UnityEngine;

public class Checkpoint : MonoBehaviour, ILDtkImportedEntity
{
    public bool IsStart;
    
    private static string LastCheckpoint;
    public string iid;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetCheckpoint()
    {
        LastCheckpoint = null;
    }

    private void Start()
    {
        if (IsStart && LastCheckpoint == null)
        {
            Player.Instance.transform.position = transform.position;
            return;
        }
        
        if (LastCheckpoint == iid)
        {
            Player.Instance.transform.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            LastCheckpoint = iid;
        }
    }

    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        iid = entityInstance.Iid;
    }
}