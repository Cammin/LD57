using System.Linq;
using CamLib;
using LDtkUnity;
using UnityEngine;

public class GhostDoor : MonoBehaviour, ILDtkImportedFields
{
    public _GhostBase[] Ghosts;

    public void OnLDtkImportFields(LDtkFields fields)
    {
        LDtkReferenceToAnEntityInstance[] refs = fields.GetEntityReferenceArray("Ghosts");
        Ghosts = refs.Select(p => p.FindEntity()).Select(p => p.GetComponent<_GhostBase>()).ToArray();
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
            Destroy(gameObject);
        }
    }
}