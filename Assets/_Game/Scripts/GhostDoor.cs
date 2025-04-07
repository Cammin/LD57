using System.Linq;
using CamLib;
using LDtkUnity;
using UnityEngine;

public class GhostDoor : MonoBehaviour, ILDtkImportedFields
{
    public GhostBase[] Ghosts;
    
    public AudioSource Sound;

    private bool Destroying;
    
    public Collider2D Collider;

    public GameObject Idle1;
    public GameObject Idle2;
    
    public GameObject Open1;
    public GameObject Open2;

    public void OnLDtkImportFields(LDtkFields fields)
    {
        LDtkReferenceToAnEntityInstance[] refs = fields.GetEntityReferenceArray("Ghosts");
        Ghosts = refs.Select(p => p.FindEntity()).Select(p => p.GetComponent<GhostBase>()).ToArray();
    }

    private void Update()
    {
        if (Ghosts.IsNullOrEmpty())
        {
            Destroy(gameObject);
        }

        bool allNull = Ghosts.All(p => p == null);
        if (allNull)
        {
            if (!Destroying)
            {
                Destroying = true;
                Sound.Play();
                
                Collider.enabled = false;
                
                Idle1.SetActive(false);
                Idle2.SetActive(false);
                Open1.SetActive(true);
                Open2.SetActive(true);
                
                Destroy(gameObject, 1f);
            }
        }
    }
}