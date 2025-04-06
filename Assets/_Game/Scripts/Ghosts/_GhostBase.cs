using Spine;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class GhostBase : MonoBehaviour
{
    public GhostBehaviour GhostBehaviour;
    public Animator Animator;
    public Rigidbody2D Rigidbody;
    [Space]
    public bool CanMove = true;
    public float MoveSpeedModifier = 1f;
    [Space]
    public float ActionCooldownDuration = 3f;
    [Space]
    public bool CanBeCaptured = true;
    public int ScoreAddedForCapture = 100;
    public float CaptureDifficultyModifier = 1f;
    public float CaptureForceModifier = 20f;
    [ColorUsageAttribute(false)]
    public Color CaptureFlashColor;
    [Space]
    public bool CanDetectPlayerThroughWalls;
    public bool RetreatIfTooClose;
    public float DetectPlayerRange = 25f;
    public float AttackPlayerRange = 20f;
    public float StopAtDistance = 3f;

    //-------------------------------------------------

    public const float DefaultSpeed = 1000f;
    public const float DefaultCaptureForce = 1000f;

    public bool CaptureInProgress => Player.Instance.GhostTarget == this;

    [NonSerialized] public Vector3 OverrideDestination;
    [NonSerialized] public float CaptureProgress;
    [NonSerialized] public bool PlayerFound;
    [NonSerialized] public bool BlockActions;
    [NonSerialized] public bool BlockMovement;
    [NonSerialized] public bool BlockCapture;
    

    [NonSerialized] private float CooldownRemaining;
    [NonSerialized] private bool Destroying;

    private void Start()
    {
        //In case it isn't assigned in the Inspector.
        GhostBehaviour = GetComponent<GhostBehaviour>();
    }

    private void Update()
    {
        if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

        if (CaptureProgress > 0) CaptureProgress -= Time.deltaTime;

        if (PlayerFound || CaptureInProgress)
        {
            transform.localScale = new Vector3(Player.Instance.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
        }

        //Ghost shouldn't be able to do anything if player is mid-capture QTE
        if (CaptureInProgress)
        {
            return;
        }

        if (!PlayerFound && Vector2.Distance(Player.Instance.transform.position, transform.position) <= DetectPlayerRange)
        {
            //If player can't be detected through walls, raycast to player to confirm no walls. If wall is found, return.
            if (!CanDetectPlayerThroughWalls)
            {
                if (CheckForWalls()) return;
            }

            PlayerFound = true;

            if (CanMove)
            {
                OverrideDestination = Vector3.zero;
            }
        }
        else if (PlayerFound && !CanDetectPlayerThroughWalls && CheckForWalls())
        {
            PlayerFound = false;
            OverrideDestination = Player.Instance.transform.position;
        }

        if (!BlockActions && !Destroying && PlayerFound && CooldownRemaining <= 0)
        {
            if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= AttackPlayerRange)
            {
                CooldownRemaining = ActionCooldownDuration;
                if (GhostBehaviour) GhostBehaviour.GhostAction();
            }
        }
    }

    public void FixedUpdate()
    {
        Rigidbody.linearVelocity = Vector2.zero;

        if (CaptureInProgress)
        {
            if (Player.Instance.CaptureQTEActive || Destroying)
            {
                var currentDistance = Vector2.Distance(Player.Instance.transform.position, transform.position);

                var distanceModifier = 100f / (Player.CaptureDistanceForQTE - 1);
                var newDistance = Player.CaptureDistanceForQTE - CaptureProgress / (distanceModifier);

                if (currentDistance > newDistance)
                {
                    var direction = (Player.Instance.transform.position - transform.position).normalized;
                    Rigidbody.AddForce(direction * currentDistance * DefaultCaptureForce * Time.deltaTime * Player.Instance.moveSpeed / 2f);
                }
            }

            if (!Player.Instance.CaptureQTEActive)
            {
                var direction = (Player.Instance.transform.position - transform.position).normalized;
                Rigidbody.AddForce(direction * CaptureForceModifier * DefaultCaptureForce * Time.deltaTime);
            }
        }
        else if (!BlockMovement)
        {
            //If destination is overriden prioritize moving to the override.
            if (OverrideDestination != Vector3.zero)
            {
                if (Vector2.Distance(OverrideDestination, transform.position) > .01f)
                {
                    var direction = (OverrideDestination - transform.position).normalized;
                    Rigidbody.AddForce(direction * MoveSpeedModifier * DefaultSpeed * Time.deltaTime);
                }
                else
                {
                    OverrideDestination = Vector3.zero;
                }
            }
            else if (PlayerFound && CanMove)
            {
                ///Move ghost towards player if stop distance hasn't been reached. If distance is less than stopping distance and
                ///RetreatIfTooClose == true, ghost should move in opposite direction of the player until the stopping distance has
                ///been reached.
                if (Vector2.Distance(Player.Instance.transform.position, transform.position) > StopAtDistance)
                {
                    var direction = (Player.Instance.transform.position - transform.position).normalized;
                    Rigidbody.AddForce(direction * MoveSpeedModifier * DefaultSpeed * Time.deltaTime);
                }
                else if (RetreatIfTooClose)
                {
                    var direction = (transform.position - Player.Instance.transform.position).normalized;
                    Rigidbody.AddForce(direction * MoveSpeedModifier * DefaultSpeed * Time.deltaTime);
                }
            }
        }
    }

    public bool CheckForWalls()
    {
        var hits = Physics2D.RaycastAll(transform.position, (Player.Instance.transform.position - transform.position).normalized, DetectPlayerRange);

        foreach(var hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) 
            {
                return false;
            } 
            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    public void StartCapture()
    {
        BlockActions = true;
        BlockCapture = true;
        BlockMovement = true;

        Animator.SetTrigger("suck");

        GameManager.Instance.CameraZoom(4);
        GameManager.Instance.OverrideCameraTarget = transform;
    }

    public void CaptureGhostAddProgress()
    {
        CaptureProgress += (Player.Instance.ProgressPerQTEHit / CaptureDifficultyModifier);

        if (CaptureProgress >= 100 && !Destroying)
        {
            CaptureGhost();
        }
    }

    public void CaptureGhost()
    {
        Destroying = true;

        Animator.SetTrigger("die");

        StartCoroutine(CoCapture());

        IEnumerator CoCapture()
        {
            yield return new WaitForSeconds(.625f);

            GameManager.Instance.ImpulseColourVolume(CaptureFlashColor);

            yield return new WaitForSeconds(.5f);

            Player.Instance.GhostTarget = null;

            GameManager.Instance.OverrideCameraTarget = null;

            Player.Instance.CaptureQTEActive = false;
            Player.Instance.AddScore(ScoreAddedForCapture);

            GameManager.Instance.CameraResetZoom();

            Destroy(gameObject);
        }
    }
}
