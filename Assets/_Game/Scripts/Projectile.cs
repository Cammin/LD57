using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //Constants
    //-----------------------------------------------
    public const float DefaultSpeed = 1000f;
    public const float CleanUpAfterDuration = 10f;
    public const float HitWallsAfterDuration = .5f;
    //-----------------------------------------------

    public Rigidbody2D Rigidbody;
    public GameObject ProjectileMoving;
    public GameObject ProjectileImpact;
    public AudioSource DestroySfx;

    public float ProjectileSpeedModifier = 5f;

    private Vector2 Trajectory;
    private float LifeTime;

    public static void SpawnProjectile(Projectile ProjectilePrefab, Vector2 StartingPosition, Vector2 TargetPosition)
    {
        var projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = StartingPosition;
        projectile.SetupProjectile(TargetPosition - StartingPosition);
    }

    private void Update()
    {
        LifeTime += Time.deltaTime;

        if (LifeTime >= CleanUpAfterDuration)
        {
            DestroyProjectile();
        }
    }

    private void FixedUpdate()
    {
        Rigidbody.linearVelocity = Vector2.zero;

        if (Trajectory != Vector2.zero)
        {
            Rigidbody.AddForce(Trajectory * (ProjectileSpeedModifier * DefaultSpeed * Time.deltaTime));
        }
    }

    public void SetupProjectile(Vector2 direction)
    {
        Trajectory = direction.normalized;

        float angle = Mathf.Atan2(Trajectory.y, Trajectory.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    //Check if we hit the player.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Trajectory == Vector2.zero) return; //We don't want anything else to get hit by the projectile if it's not moving.

        if (other.gameObject.layer == LayerMask.NameToLayer("Ghost") || other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            return;
        }

        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            if (player.TryTakeDamage())
            {
                Debug.Log("Player hit!");
            }
        }
        else if (LifeTime < HitWallsAfterDuration)
        {
            return;
        }

        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        Trajectory = Vector2.zero; //Set trajectory to 0 so that we aren't triggering anything else by accident during destroy routine.
        StartCoroutine(CoDestroy());

        IEnumerator CoDestroy()
        {
            ProjectileMoving.SetActive(false);
            ProjectileImpact.SetActive(true);
            DestroySfx.Play();

            yield return new WaitForSeconds(1f);

            Destroy(gameObject);
        }
    }
}
