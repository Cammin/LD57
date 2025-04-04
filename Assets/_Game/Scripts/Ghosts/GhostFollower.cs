using System.Collections;
using UnityEngine;
public class GhostFollower : GhostScript
{
    public Projectile ProjectilePrefab;

    public override void GhostAction()
    {
        base.GhostAction();

        //TODO animate
        StartCoroutine(CoCastProjectile());

        IEnumerator CoCastProjectile()
        {
            yield return new WaitForSeconds(1);
            Projectile.SpawnProjectile(ProjectilePrefab, transform.position, Player.Instance.transform.position);
        }
    }
}