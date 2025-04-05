using CamLib;
using UnityEngine;

public class SortZ : MonoBehaviour
{
    public float yPointOffset = 0.0f;
    
    private void LateUpdate()
    {
        float y = transform.position.y + yPointOffset;
        float z = y * 0.001f;
        transform.SetZ(z);
    }

    private void OnDrawGizmosSelected()
    {
        //light blue
        Gizmos.color = new Color(0.3f, 0.7f, 1.0f, 1f);
        Gizmos.DrawSphere(transform.position + Vector3.up * yPointOffset, 0.05f);
    }
}