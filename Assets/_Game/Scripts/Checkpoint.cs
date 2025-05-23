using LDtkUnity;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoint : MonoBehaviour, ILDtkImportedEntity, ILDtkImportedFields
{
    public string iid;
    public bool IsStart;
    public bool InvincibleBatteryOnSpawn;
    [Space]
    public AudioSource Drink;
    public Animator Anim;
    public Light2D TurnOffLightUponCollect;
    
    private static string LastCheckpoint;
    private bool Touched;

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

            if (!InvincibleBatteryOnSpawn)
            {
                MusicManager.Instance.ChangeToMusic();
            }
            
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Touched) return;

        Player player = other.GetComponentInParent<Player>(other);
        if (player)
        {
            LastCheckpoint = iid;
            Touched = true;

            if (!IsStart)
            {
                Drink.Play();
                Anim.SetTrigger("collect");
                GameManager.Instance.ImpulseColourVolume(Color.green);
                GameManager.Instance.CreatePulseText("Checkpoint!", Color.green);
                TurnOffLightUponCollect.enabled = false;
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
        if (fields.TryGetBool("InvincibleBatteryOnSpawn", out var inv))
        {
            InvincibleBatteryOnSpawn = inv;
        }
    }
}