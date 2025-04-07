using CamLib;
using System.Collections;
using LDtkUnity;
using UnityEngine;

public class GhostTransparent : GhostBehaviour, ILDtkImportedEntity
{
    public Rect Restrict;
    
    public void Update()
    {
        Ghost.BlockMovement = Player.Instance.flashlight.IsInLight(transform.position);
        
        Ghost.Animator.SetBool("Freeze", Ghost.BlockMovement);
        
        //restrict position inside the level's rect
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

    public void OnLDtkImportEntity(EntityInstance entityInstance)
    {
        LDtkComponentEntity entity = GetComponent<LDtkComponentEntity>();
        LDtkComponentLevel level = entity.Parent.Parent;
        Restrict = level.BorderRect;
    }
}