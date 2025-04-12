using CamLib;
using System.Collections;
using LDtkUnity;
using UnityEngine;

//The Transparent ghost has a melee attack that it can use when it gets into range of the player and cannot be captured. Instead, the 
//player can use the flashlight to cause the ghost to freeze in place. Inspired by the Boos in Super Mario freezing in place when you
//look in their direction.
public class GhostTransparent : GhostBehaviour, ILDtkImportedEntity
{
    public Rect Restrict;
    
    public void Update()
    {
        bool shone = Player.Instance.Flashlight.IsInLight(transform.position);

        Ghost.BlockMovement = shone;
        Ghost.BlockActions = shone;
        
        Ghost.Animator.SetBool("Freeze", Ghost.BlockMovement);
        
        //Restrict position inside the level's Rect.
        if (Restrict != Rect.zero)
        {
            Vector2 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, Restrict.xMin, Restrict.xMax);
            pos.y = Mathf.Clamp(pos.y, Restrict.yMin, Restrict.yMax);
            transform.position = pos;
        }
    }

    public override void GhostAction()
    {
        base.GhostAction();

        StartCoroutine(CoAttack());

        IEnumerator CoAttack()
        {
            Ghost.Animator.SetTrigger("attack");
            yield return new WaitForSeconds(0.45f);

            if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= Ghost.AttackPlayerRange)
            {
                Player.Instance.TryTakeDamage();
            }
        }
    }

    //Only begin chasing the player if they've entered the same level as the ghost.
    public override bool FindPlayerConditions()
    {
        return Restrict.Contains(Player.Instance.transform.position);
    }

    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        LDtkComponentEntity entity = GetComponent<LDtkComponentEntity>();
        LDtkComponentLevel level = entity.Parent.Parent;
        Restrict = level.BorderRect;
    }
}