using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed class GhostBase : MonoBehaviour
{
    //Constants
    //---------------------------------------------------
    public const float DefaultSpeed = 1000f;
    public const float DefaultCaptureForce = 1000f;
    public const float CaptureProgressDecay = 5f;
    //---------------------------------------------------

    public bool CaptureInProgress => Player.Instance.GhostTarget == this;

    public GhostBehaviour GhostBehaviour;
    public Animator Animator;
    public Rigidbody2D Rigidbody;
    public Light2D AmbientLight;
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
    [ColorUsageAttribute(false)] public Color CaptureFlashColor = Color.white;
    [Space]
    public bool CanDetectPlayerThroughWalls;
    public bool RetreatIfTooClose;
    public float DetectPlayerRange = 25f;
    public float AttackPlayerRange = 20f;
    public float StopAtDistance = 3f;
    [Space]
    public AudioSource SfxSuckStart;
    public AudioSource SfxSuckDefeat;
    public AudioSource SfxShoot;

    [NonSerialized] public Vector3 OverrideDestination;
    [NonSerialized] public float CaptureProgress;
    [NonSerialized] public bool PlayerFound;
    [NonSerialized] public bool BlockActions;
    [NonSerialized] public bool BlockMovement;
    [NonSerialized] public bool BlockCapture;

    private float CooldownRemaining;
    private bool Destroying;
    
    private void Start()
    {
        //In case it isn't assigned in the Inspector.
        GhostBehaviour = GetComponent<GhostBehaviour>();
    }

    private void Update()
    {
        if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

        if (CaptureProgress > 0) CaptureProgress -= CaptureProgressDecay * Time.deltaTime;

        //Facing direction
        if (OverrideDestination != Vector3.zero)
        {
            transform.localScale = new Vector3(OverrideDestination.x > transform.position.x ? 1 : -1, 1, 1);
        }
        else if (PlayerFound || CaptureInProgress)
        {
            transform.localScale = new Vector3(Player.Instance.transform.position.x > transform.position.x ? 1 : -1, 1, 1);
        }

        //Ghost shouldn't be able to do anything if player is mid-capture QTE.
        if (CaptureInProgress)
        {
            return;
        }

        //If the player hasn't been found but they're within range.
        if (!PlayerFound && Vector2.Distance(Player.Instance.transform.position, transform.position) <= DetectPlayerRange)
        {
            if (GhostBehaviour)
            {
                if (!GhostBehaviour.FindPlayerConditions()) return;
            }

            //If player can't be detected through walls, raycast to player to confirm no walls. If wall is found, return.
            if (!CanDetectPlayerThroughWalls)
            {
                if (CheckForWalls()) return;
                if (!IsInsideCamera()) return;
            }

            PlayerFound = true;

            if (CanMove)
            {
                OverrideDestination = Vector3.zero;
            }
        }
        else if (PlayerFound && ((!CanDetectPlayerThroughWalls && CheckForWalls()) || (GhostBehaviour && !GhostBehaviour.FindPlayerConditions())))
        {
            PlayerFound = false;

            //Ghost travels to the player's last known position if they lose sight of them.
            if (CanMove) OverrideDestination = Player.Instance.transform.position;
        }

        //Ghost does action, as defined by the assigned GhostBehaviour.
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

        //Physics manipulation due to capture mechanic takes priority.
        if (CaptureInProgress)
        {
            if (Player.Instance.CaptureQTEActive || Destroying)
            {
                var currentDistance = Vector2.Distance(Player.Instance.transform.position, transform.position);

                var distanceModifier = 100f / (Player.CaptureDistanceForQTE - 1);
                var newDistance = 2f + Player.CaptureDistanceForQTE - CaptureProgress / (distanceModifier);

                if (currentDistance > newDistance)
                {
                    var direction = (Player.Instance.transform.position - transform.position).normalized;
                    Rigidbody.AddForce(direction * (currentDistance * DefaultCaptureForce * Time.deltaTime * Player.Instance.MoveSpeed) / 2f);
                }
            }

            if (!Player.Instance.CaptureQTEActive)
            {
                var direction = (Player.Instance.transform.position - transform.position).normalized;
                Rigidbody.AddForce(direction * (CaptureForceModifier * DefaultCaptureForce * Time.deltaTime));
            }
        }
        else if (!BlockMovement)
        {
            //If destination is overriden prioritize moving to the override.
            if (OverrideDestination != Vector3.zero)
            {
                if (Vector2.Distance(OverrideDestination, transform.position) > .1f)
                {
                    var direction = (OverrideDestination - transform.position).normalized;
                    Rigidbody.AddForce(direction * (MoveSpeedModifier * DefaultSpeed * Time.deltaTime));
                }
                else
                {
                    transform.position = OverrideDestination;
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
                    Rigidbody.AddForce(direction * (MoveSpeedModifier * DefaultSpeed * Time.deltaTime));
                }
                else if (RetreatIfTooClose)
                {
                    var direction = (transform.position - Player.Instance.transform.position).normalized;
                    Rigidbody.AddForce(direction * (MoveSpeedModifier * DefaultSpeed * Time.deltaTime));
                }
            }
        }
    }

    //In order to check if there's a wall between the ghost and the player we do a raycast, and if the wall is earlier in the list than
    //the player we return true.
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

    //Override method where we pass a custom range as an argument instead of the ghost class's pre-defined detection range.
    public bool CheckForWalls(float customRange)
    {
        var hits = Physics2D.RaycastAll(transform.position, (Player.Instance.transform.position - transform.position).normalized, customRange);

        foreach (var hit in hits)
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
        SfxSuckStart.Play();

        GameManager.Instance.CameraZoom(7f);
        GameManager.Instance.OverrideCameraTarget = transform;

        Hud.Instance.ShowSpacebar(true);
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
        SfxSuckDefeat.Play();

        Hud.Instance.ShowSpacebar(false);

        StartCoroutine(CoCapture());

        IEnumerator CoCapture()
        {
            yield return new WaitForSeconds(.625f);

            GameManager.Instance.ImpulseColourVolume(CaptureFlashColor);
            
            if (AmbientLight) AmbientLight.enabled = false;

            yield return new WaitForSeconds(.5f);

            Player.Instance.GhostTarget = null;

            GameManager.Instance.OverrideCameraTarget = null;

            Player.Instance.CaptureQTEActive = false;
            Player.Instance.AddScore(ScoreAddedForCapture);

            GameManager.Instance.CameraResetZoom();

            Destroy(gameObject, 1);
        }
    }
    
    public bool IsInsideCamera()
    {
        return GameManager.IsPointInsideCamera(Player.Instance.GetCamera(), transform.position);
    }

    public bool IsBeingDestroyed()
    {
        return Destroying;
    }
}
