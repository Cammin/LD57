using CamLib;
using System.Collections;
using UnityEngine;

public class GhostTransparent : GhostBehaviour
{
    public void Update()
    {
        Ghost.BlockMovement = Player.Instance.flashlight.IsInLight(transform.position);
    }

    public override void GhostAction()
    {
        base.GhostAction();

        //TODO animation

        StartCoroutine(CoAttack());

        IEnumerator CoAttack()
        {
            yield return new WaitForSeconds(1); //TODO anim time

            if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= Ghost.AttackPlayerRange)
            {
                Player.Instance.TryTakeDamage();
            }
        }
    }
}