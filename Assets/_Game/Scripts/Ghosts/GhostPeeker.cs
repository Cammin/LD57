using System.Collections.Generic;
using System.Linq;
using LDtkUnity;

public class GhostPeeker : GhostScript, ILDtkImportedFields
{ 
    //Look down a corner. if it sees the player, shoot a 3 rounds burst of projectiles per interval then hide back behind it

    public PeekerHidingSpot[] hidingSpots;
    public PeekerHidingSpot CurrentHidingSpot;
    
    public void OnLDtkImportFields(LDtkFields fields)
    {
        LDtkReferenceToAnEntityInstance[] refs = fields.GetEntityReferenceArray("HideSpots");
        IEnumerable<LDtkIid> iids = refs.Select(p => p.GetEntity());
        hidingSpots = iids.Select(p => p.GetComponent<PeekerHidingSpot>()).ToArray();
    }
}