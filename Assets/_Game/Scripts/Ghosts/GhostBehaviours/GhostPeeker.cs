using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LDtkUnity;
using UnityEngine;

//Look down a corner. if it sees the player, shoot a X round burst of projectiles per interval then hide back behind it.
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

    [NonSerialized] private bool Retreating;
    [NonSerialized] private int TimesRetreated;

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
            Debug.Log("Reset");
            Retreating = false;
            Ghost.BlockActions = false;
            Ghost.BlockCapture = false;
        }

        if (!Ghost.BlockActions && TimesRetreated < TimesAllowedToRetreat && Vector3.Distance(Player.Instance.transform.position, transform.position) <= RetreatDistance)
        {
            RetreatToNewPoint();
        }
    }

    public override void GhostAction()
    {
        base.GhostAction();

        //TODO animate
        StartCoroutine(CoCastProjectileBurst());

        IEnumerator CoCastProjectileBurst()
        {
            yield return new WaitForSeconds(1); //TODO change to anim time

            for(int i = 0; i < ProjectileBurstHowMany; i++)
            {
                Projectile.SpawnProjectile(ProjectilePrefab, transform.position, Player.Instance.transform.position);

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

    public void RetreatToNewPoint()
    {
        Debug.Log("Retreat start");

        Ghost.BlockActions = true;
        Ghost.BlockCapture = true;

        StartCoroutine(CoRetreat());

        IEnumerator CoRetreat()
        {
            //TODO play animation

            yield return new WaitForSeconds(1); //TODO change to anim time

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
}