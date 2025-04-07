using System.Collections;
using UnityEngine;

public class GhostStationary : GhostBehaviour
{
    public Projectile ProjectilePrefab;

    public override void GhostAction()
    {
        base.GhostAction();

        Ghost.Animator.SetTrigger("shoot");
        Ghost.SfxShoot.Play();
        
        StartCoroutine(CoCastProjectile());

        IEnumerator CoCastProjectile()
        {
            Ghost.BlockMovement = true;

            yield return new WaitForSeconds(.3f);

            if (Ghost.CaptureInProgress)
                yield break;

            Projectile.SpawnProjectile(ProjectilePrefab, transform.position, Player.Instance.transform.position);

            yield return new WaitForSeconds(.2f);

            if (Ghost.CaptureInProgress)
                yield break;

            Ghost.BlockMovement = false;
        }
    }
}