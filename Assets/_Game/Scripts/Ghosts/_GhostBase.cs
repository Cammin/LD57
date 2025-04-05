using System;
using UnityEngine;
using UnityEngine.UIElements;

public class _GhostBase : MonoBehaviour
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
    public float DetectPlayerRange = 5f;
    public float StopAtDistance = 1f;

    //-------------------------------------------------

    public const int WallLayer = 8;
    public const float DefaultSpeed = 1000f;

    [NonSerialized] public bool PlayerFound;
    [NonSerialized] public bool CaptureInProgress;
    [NonSerialized] private float CooldownRemaining;


    private void Start()
    {

    }

    private void Update()
    {
        if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

        if (CaptureInProgress) return;

        if (!PlayerFound && Vector2.Distance(Player.Instance.transform.position, transform.position) <= DetectPlayerRange)
        {
            if (!CanDetectPlayerThroughWalls)
            {
                var hit = Physics2D.Raycast(transform.position, (Player.Instance.transform.position - transform.position).normalized, DetectPlayerRange);

                if (hit.collider != null && hit.collider.gameObject.layer == WallLayer)
                {
                    return;
                }
                else
                {
                    Debug.Log("Player found!");
                }
            }

            PlayerFound = true;
        }

        if (PlayerFound && CooldownRemaining <= 0)
        {
            CooldownRemaining = ActionCooldownDuration;
            GhostAction();
        }
    }

    public void FixedUpdate()
    {
        Rigidbody.linearVelocity = Vector2.zero;

        if (PlayerFound && CanMove)
        {
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

    public virtual void GhostAction()
    {
        Debug.Log("Ghost action!");
    }
}
