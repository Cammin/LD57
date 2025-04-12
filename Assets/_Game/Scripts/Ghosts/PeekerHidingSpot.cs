using LDtkUnity;
using UnityEngine;

public class PeekerHidingSpot : MonoBehaviour, ILDtkImportedFields
{
    public Vector2[] Points;
    
    public void OnLDtkImportFields(LDtkFields fields)
    {
        Points = fields.GetPointArray("HideSpots");
    }

    public Vector2 GetFurthestPoint(Vector2 from)
    {
        Vector2 furthestPoint = Points[0];
        float maxDistance = Vector2.Distance(from, Points[0]);

        foreach (var point in Points)
        {
            float distance = Vector2.Distance(from, point);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestPoint = point;
            }
        }
        return furthestPoint;
    }
}