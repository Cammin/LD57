using LDtkUnity;
using UnityEngine;

public class ClockOutStation : MonoBehaviour, ILDtkImportedFields
{
    public bool PlayerInRange;
    public ExitDoor Door;

    public AudioSource Sound;
    
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
        Sound.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        LDtkReferenceToAnEntityInstance ent = fields.GetEntityReference("Door");
        Door = ent.GetEntity().GetComponent<ExitDoor>();
    }
}