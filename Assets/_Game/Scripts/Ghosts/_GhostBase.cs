using System;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class _GhostBase : MonoBehaviour
{
    public Rigidbody2D Rigidbody;
    [Space]
    public bool CanMove = true;
    public float MoveSpeedModifier = 1f;
    [Space]
    public float ActionCooldownDuration = 3f;
    [Space]
    public bool CanBeCaptured = true;
    public float CaptureDifficultyModifier = 1f;
    [Space]
    public bool CanDetectPlayerThroughWalls;
    public bool RetreatIfTooClose;
    public float DetectPlayerRange = 25f;
    public float AttackPlayerRange = 20f;
    public float StopAtDistance = 3f;

    public GhostBehaviour GhostBehaviour;
    
    //-------------------------------------------------

    public const float DefaultSpeed = 1000f;

    [NonSerialized] public Vector3 OverrideDestination;
    [NonSerialized] public bool PlayerFound;
    [NonSerialized] public bool CaptureInProgress;
    [NonSerialized] public bool BlockActions;
    [NonSerialized] public bool BlockCapture;

    [NonSerialized] private float CooldownRemaining;


    private void Start()
    {
        //In case it isn't assigned in the Inspector.
        GhostBehaviour = GetComponent<GhostBehaviour>();
    }

    private void Update()
    {
        if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

        //Ghost shouldn't be able to do anything if player is mid-capture QTE
        if (CaptureInProgress) return;

        if (!PlayerFound && Vector2.Distance(Player.Instance.transform.position, transform.position) <= DetectPlayerRange)
        {
            //If player can't be detected through walls, raycast to player to confirm no walls. If wall is found, return.
            if (!CanDetectPlayerThroughWalls)
            {
                if (CheckForWalls()) return;
            }

            PlayerFound = true;
        }
        else if (PlayerFound && !CanDetectPlayerThroughWalls && CheckForWalls())
        {
            PlayerFound = false;
            OverrideDestination = Player.Instance.transform.position;
        }

        if (!BlockActions && PlayerFound && CooldownRemaining <= 0)
        {
            if (Vector2.Distance(Player.Instance.transform.position, transform.position) <= AttackPlayerRange)
            {
                CooldownRemaining = ActionCooldownDuration;
                GhostBehaviour.GhostAction();
            }
        }
    }

    public void FixedUpdate()
    {
        Rigidbody.linearVelocity = Vector2.zero;

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
        else
        {
            Debug.Log($"{gameObject.name}: Not moving");
        }
    }

    public bool CheckForWalls()
    {
        var hit = Physics2D.Raycast(transform.position, (Player.Instance.transform.position - transform.position).normalized, DetectPlayerRange);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            return true;
        }

        return false;
    }
}
