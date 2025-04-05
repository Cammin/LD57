using UnityEngine;

public abstract class GhostScript : MonoBehaviour
{
    public virtual void GhostAction()
    {
        Debug.Log("Ghost action!");
    }
}