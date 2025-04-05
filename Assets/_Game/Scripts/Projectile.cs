using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D Rigidbody;
    public float ProjectileSpeedModifier = 5f;

    public const float DefaultSpeed = 1000f;

    [NonSerialized] public Vector2 Trajectory;

    public static void SpawnProjectile(Projectile ProjectilePrefab, Vector2 StartingPosition, Vector2 TargetPosition)
    {
        var projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = StartingPosition;
        projectile.SetupProjectile(TargetPosition - StartingPosition);
    }

    private void FixedUpdate()
    {
        Rigidbody.linearVelocity = Vector2.zero;

        if (Trajectory != Vector2.zero)
        {
            Rigidbody.AddForce(Trajectory * ProjectileSpeedModifier * DefaultSpeed * Time.deltaTime);
        }
    }

    public void SetupProjectile(Vector2 direction)
    {
        Trajectory = direction.normalized;
    }

    //Check hit
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Trajectory == Vector2.zero) return; //We don't want anything else to get hit by the projectile if it's not moving

        if (other.gameObject.layer == LayerMask.NameToLayer("Ghost") || other.gameObject.layer == LayerMask.NameToLayer("Projectile")) return;

        if (other.TryGetComponent<Player>(out var player))
        {
            if (player.TryTakeDamage())
            {
                Debug.Log("Player hit!");
            }
        }

        Destroy(gameObject);
    }

    public void DestroyProjectile()
    {
        Trajectory = Vector2.zero; //Set trajectory to 0 so that we aren't triggering anything else by accident during destroy routine
        StartCoroutine(CoDestroy());

        IEnumerator CoDestroy()
        {
            //TODO play animation

            yield return new WaitForSeconds(1); //TODO change to animation time

            Destroy(gameObject);
        }
    }
}
