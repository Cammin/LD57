using System;
using UnityEngine;

public class _GhostBase : MonoBehaviour
{
    public bool CanMove = true;
    public float MoveSpeedModifier = 1f;
    [Space]
    public float ActionCooldownDuration = 3f;
    [Space]
    public bool CanBeCaptured = true;
    public float CaptureDifficultyModifier = 1f;
    [Space]
    public float DetectPlayerRange = 2f;

    [NonSerialized] public bool CaptureInProgress;
    [NonSerialized] private float CooldownRemaining;

    private void Start()
    {

    }

    private void Update()
    {
        if (CooldownRemaining > 0) CooldownRemaining -= Time.deltaTime;

        if (CaptureInProgress) return;

        OnGhostUpdate();
    }

    public virtual void OnGhostUpdate()
    {

    }
}
