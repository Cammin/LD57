using UnityEngine;

public abstract class GhostBehaviour : MonoBehaviour
{
    public GhostBase Ghost;

    public virtual void Start()
    {
        //In case it isn't assigned in the Inspector.
        Ghost = GetComponent<GhostBase>();
    }

    public virtual void GhostAction()
    {
        Debug.Log("Ghost action!");
    }

    public virtual bool FindPlayerConditions()
    {
        return true;
    }
}