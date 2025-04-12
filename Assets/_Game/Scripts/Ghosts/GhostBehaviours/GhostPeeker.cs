using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LDtkUnity;
using UnityEngine;

/// <summary>
/// Look down a corner. if it sees the player, shoot a X round burst of projectiles per interval then hide back behind it.
/// </summary>
public class GhostPeeker : GhostBehaviour, ILDtkImportedFields
{
    public Projectile ProjectilePrefab;
    public int ProjectileBurstHowMany = 3;
    public float TimeBetweenProjectiles = .2f;
    [Space]
    public PeekerHidingSpot[] HidingSpots;
    public PeekerHidingSpot CurrentHidingSpot;
    [Space]
    public float RetreatDistance = 5f;
    public int TimesAllowedToRetreat = 1;
    [Space]
    public AudioSource SfxPeekerHide;
    public AudioSource SfxPeekerReveal;
    
    private bool Retreating;
    private int TimesRetreated;

    public override void Start()
    {
        base.Start();

        CurrentHidingSpot = HidingSpots[0];
        transform.position = CurrentHidingSpot.GetFurthestPoint(Player.Instance.transform.position);
    }

    private void Update()
    {
        if (Ghost.OverrideDestination == Vector3.zero && Retreating)
        {
            RetreatEnd();
        }

        if (!Ghost.BlockActions && TimesRetreated < TimesAllowedToRetreat && 
                                Vector3.Distance(Player.Instance.transform.position, transform.position) <= RetreatDistance)
        {
            RetreatToNewPoint();
        }
    }

    public override void GhostAction()
    {
        base.GhostAction();

        Ghost.Animator.SetTrigger("shoot");
        Ghost.SfxShoot.Play();
        StartCoroutine(CoCastProjectileBurst());

        IEnumerator CoCastProjectileBurst()
        {
            yield return new WaitForSeconds(.3f);

            for(int i = 0; i < ProjectileBurstHowMany; i++)
            {
                if (Ghost.CaptureInProgress)
                    yield break;

                Projectile.SpawnProjectile(ProjectilePrefab, transform.position + Vector3.up * 0.8f, Player.Instance.transform.position);

                yield return new WaitForSeconds(TimeBetweenProjectiles);
            }
        }
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        LDtkReferenceToAnEntityInstance[] refs = fields.GetEntityReferenceArray("HideSpots");
        IEnumerable<LDtkIid> iids = refs.Select(p => p.GetEntity());
        HidingSpots = iids.Select(p => p.GetComponent<PeekerHidingSpot>()).ToArray();
    }

    /// <summary>
    /// Ghost retreats to the furthest pre-defined point from the player. Is invincible until reaching the override destination but is unable to act against the player during retreat.
    /// </summary>
    private void RetreatToNewPoint()
    {
        Ghost.BlockActions = true;
        Ghost.BlockCapture = true;

        Debug.Log("retreat");

        Ghost.Animator.SetTrigger("shadow");
        SfxPeekerHide.Play();
        StartCoroutine(CoRetreat());

        IEnumerator CoRetreat()
        {
            yield return new WaitForSeconds(.792f);

            PeekerHidingSpot furthest = null;

            foreach (var hidingSpot in HidingSpots)
            {
                if (hidingSpot == CurrentHidingSpot) continue;

                if (!furthest || Vector2.Distance(furthest.transform.position, Player.Instance.transform.position) <
                                 Vector2.Distance(hidingSpot.transform.position, Player.Instance.transform.position))
                {
                    furthest = hidingSpot;
                }
            }

            CurrentHidingSpot = furthest;

            TimesRetreated++;

            Ghost.OverrideDestination = CurrentHidingSpot.GetFurthestPoint(Player.Instance.transform.position);
            Retreating = true;
        }
    }

    private void RetreatEnd()
    {
        Retreating = false;

        Ghost.Animator.SetTrigger("revert");
        SfxPeekerReveal.Play();
        StartCoroutine(CoRetreatEnd());

        IEnumerator CoRetreatEnd()
        {
            yield return new WaitForSeconds(.792f);

            Ghost.BlockActions = false;
            Ghost.BlockCapture = false;
        }
    }
}