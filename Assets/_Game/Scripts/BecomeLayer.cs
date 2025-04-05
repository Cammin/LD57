using UnityEngine;
using UnityEngine.Tilemaps;

public class BecomeLayer : MonoBehaviour
{
    public LayerMask mask;
        
    private void Start()
    {
        foreach (TilemapCollider2D tilemap in GetComponentsInChildren<TilemapCollider2D>())
        {
            tilemap.gameObject.layer = mask;
        }
    }
}