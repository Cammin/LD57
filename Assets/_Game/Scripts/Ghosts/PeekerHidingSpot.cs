using LDtkUnity;
using UnityEngine;

public class PeekerHidingSpot : MonoBehaviour, ILDtkImportedFields
{
    public Vector2[] points;
    
    public void OnLDtkImportFields(LDtkFields fields)
    {
        points = fields.GetPointArray("HideSpots");
    }

    public Vector2 GetFurthestPoint(Vector2 from)
    {
        Vector2 furthestPoint = points[0];
        float maxDistance = Vector2.Distance(from, points[0]);

        foreach (var point in points)
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