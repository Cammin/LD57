using LDtkUnity;
using UnityEngine;

public class Checkpoint : MonoBehaviour, ILDtkImportedEntity, ILDtkImportedFields
{
    public bool IsStart;

    private static string LastCheckpoint;
    public string iid;
    public bool InvincibleBatteryOnSpawn;

    public AudioSource Drink;
    public Animator Anim;

    public bool Touched;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetCheckpoint()
    {
        LastCheckpoint = null;
    }

    private void Start()
    {
        // put player to spawn, assuming we have no stored location
        if (IsStart && LastCheckpoint == null)
        {
            Player.Instance.transform.position = transform.position;
            return;
        }

        //set player to alternate respawn point
        if (LastCheckpoint == iid)
        {
            Player.Instance.transform.position = transform.position;
            Player.Instance.IsDrainingFlashlight = !InvincibleBatteryOnSpawn;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Touched) return;

        if (other.TryGetComponent<Player>(out var player))
        {
            LastCheckpoint = iid;
            Touched = true;

            if (!IsStart)
            {
                Drink.Play();
                Anim.SetTrigger("collect");
                GameManager.Instance.ImpulseColourVolume(Color.green);
                GameManager.Instance.CreatePulseText("Checkpoint!", Color.green);
                Destroy(gameObject, 4);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        iid = entityInstance.Iid;
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        fields.TryGetBool("InvincibleBatteryOnSpawn", out InvincibleBatteryOnSpawn);
    }
}