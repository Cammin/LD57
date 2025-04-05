using UnityEngine;

public abstract class GhostBehaviour : MonoBehaviour
{
    public _GhostBase Ghost;

    public virtual void Start()
    {
        //In case it isn't assigned in the Inspector.
        Ghost = GetComponent<_GhostBase>();
    }

    public virtual void GhostAction()
    {
        Debug.Log("Ghost action!");
    }
}