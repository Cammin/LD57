using CamLib;
using System.Collections;
using UnityEngine;

public class GhostTransparent : GhostBehaviour
{
    public void Update()
    {
        Ghost.BlockMovement = Player.Instance.flashlight.IsInLight(transform.position);
        
        Ghost.Animator.SetBool("Freeze", Ghost.BlockMovement);
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
}