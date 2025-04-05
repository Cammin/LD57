using UnityEngine;
using UnityEngine.Tilemaps;

public class BecomeLayer : MonoBehaviour
{
    private void Start()
    {
        foreach (TilemapCollider2D tilemap in GetComponentsInChildren<TilemapCollider2D>())
        {
            tilemap.gameObject.layer = LayerMask.NameToLayer("Wall");
        }
    }
}