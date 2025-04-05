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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up * yPointOffset, 0.1f);
    }
}