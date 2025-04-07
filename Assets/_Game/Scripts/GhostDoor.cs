using System.Collections;
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

    public Transform CameraTrack;

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
                StartCoroutine(DoOpen());
            }
        }
    }

    private IEnumerator DoOpen()
    {
        bool shouldPan = !GameManager.IsPointInsideCamera(CameraTrack.position);

        if (shouldPan)
        {
            GameManager.Instance.OverrideCameraTarget = CameraTrack; 
            yield return new WaitForSeconds(1.0f);
        }
        
        Sound.Play();
                
        Collider.enabled = false;
        Idle1.SetActive(false);
        Idle2.SetActive(false);
        Open1.SetActive(true);
        Open2.SetActive(true);

        if (shouldPan)
        {
            yield return new WaitForSeconds(1.1f);
            GameManager.Instance.OverrideCameraTarget = null; 
        }
        
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}