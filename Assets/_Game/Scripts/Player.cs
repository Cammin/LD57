using CamLib;
using DG.Tweening;
using Spine.Unity;
using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    //Constants
    //----------------------------------------------
    public const int DefaultHealth = 3;
    public const float CaptureCastRadius = 1f;
    public const float CaptureCastLength = 10f;
    public const float CaptureDistanceForQTE = 4f;
    //----------------------------------------------

    public bool IsEmpty => BatteryLifeRemaining <= 0;
    public bool IsDead => HP <= 0;
    public float BatteryRatio => BatteryLifeRemaining / MaxBatteryLife;

    public static int Score;

    public Animator Anim;
    public SkeletonRenderer Skeleton;
    public Rigidbody2D Rigidbody;
    public GameObject Model;
    public GameObject BatteryLifeLowIndicator;
    [Space]
    public float MoveSpeed;
    public float MaxBatteryLife;
    public float ProgressPerQTEHit = 15f;
    [Space]
    public Light2D Flashlight;
    public Light2D LightAround;
    public Light2D LightStylish;
    public Vector2 LightInnerOverLifetimeRatio;
    public Vector2 LightOuterOverLifetimeRatio;
    public ParticleSystem CaptureParticles;
    public CinemachineImpulseSource Impulse;
    [Space]
    public Transform PulseTextSpawnPoint;
    public TMP_Text DialogueText;
    [Space]
    public AudioSource SfxFootstep;
    public AudioSource SfxHurt;
    public AudioSource SfxImpact;
    public AudioSource SfxDeath;
    public AudioSource SfxFlashlightEmpty;
    public AudioSource SfxQteMash;
    public AudioSource SfxVacuumLoop;

    [NonSerialized] public GhostBase GhostTarget;
    [NonSerialized] public bool CaptureActive;
    [NonSerialized] public bool CaptureQTEActive;
    [NonSerialized] public bool IsDrainingFlashlight;
    [NonSerialized] public bool IsEmptyLastFrame;
    [NonSerialized] public float InvincibilityTime;

    private Camera Camera;
    private Tween VacuumTween;
    private Tween DialogueTween;
    private Vector2 MoveInput;
    private Vector2 AimDirection;
    private bool PrevParticlesState;
    private int HP;
    private float BatteryLifeRemaining;

    private void Start()
    {
        Camera = Camera.main;
        HP = DefaultHealth;
        BatteryLifeRemaining = MaxBatteryLife;
        DialogueText.color = new Color(1, 1, 1, 0); 
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return; //If the game is paused don't do any of this.
        
        Anim.SetBool("Flashlight", !IsEmpty);
        
        if (IsDead) return;

        bool newParticlesState = CaptureActive || CaptureQTEActive;
        if (newParticlesState != PrevParticlesState)
        {
            PrevParticlesState = newParticlesState;
            if (newParticlesState)
            {
                CaptureParticles.Play();
            }
            else
            {
                CaptureParticles.Stop();
            }
        }

        Flashlight.color = CaptureActive ? Color.cyan : Color.white;

        DoMoveInput();
        TryCheats();

        //If in the middle of a QTE we don't want the rest of the update to happen.
        if (CaptureQTEActive)
        {
            if (GhostTarget)
            {
                UpdateFlashlight(GhostTarget.transform.position - transform.position);

                //Originally only allowed for spacebar to be used here, but added left click as another option for the QTE as a QoL change. Not reflected in UI.
                if (!GhostTarget.IsBeingDestroyed() && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
                {
                    GhostTarget.CaptureGhostAddProgress();
                    SfxQteMash.Play();
                }
            }

            return;
        }

        UpdateFlashlight();
        TickBatteryLifetime();
        
        InvincibilityTime -= Time.deltaTime;

        DoCaptureInput();
    }

    private void FixedUpdate()
    {
        Rigidbody.linearVelocity = MoveInput * (MoveSpeed);
    }

    public Camera GetCamera()
    {
        return Camera;
    }

    private void DoMoveInput()
    {
        //Checks for player movement input.
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(x, y, 0).normalized;
        MoveInput = move;

        Anim.SetBool("Moving", MoveInput.magnitude > 0);
    }

    public int GetHP()
    {
        return HP;
    }

    public void SetHP(int newHP)
    {
        HP = newHP;
    }

    /// <summary>
    /// Attempts to deal damage to the player. Will return true if successful. Will return false if the player is dead or has I-frames.
    /// </summary>
    /// <returns></returns>
    public bool TryTakeDamage()
    {
        if (IsDead) return false;

        if (InvincibilityTime > 0) return false;
        InvincibilityTime = 0.5f;

        SetHP(HP - 1);
        Impulse.GenerateImpulse();
        GameManager.Instance.ImpulseColourVolume(Color.red);

        SfxImpact.Play();

        if (HP <= 0)
        {
            Debug.Log("Player is dead");
            Anim.SetTrigger("death");
            MoveInput = Vector2.zero;
            Rigidbody.linearVelocity = Vector2.zero;
            SfxDeath.Play();

            FadeManager.Instance.FadeIn(Color.black, () =>
            {
                SceneManager.LoadScene("Gameplay");
            });
        }
        else
        {
            // Handle player taking damage
            Debug.Log("Player took damage, remaining HP: " + HP);
            Anim.SetTrigger("damage");
            SfxHurt.Play();
        }

        return true;
    }

    public void AddScore(int score)
    {
        GameManager.Instance.CreatePulseText($"+{score} score", Color.white);

        Score += score;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetScore()
    {
        Score = 0;
    }

    public void RechargeBattery()
    {
        BatteryLifeRemaining = MaxBatteryLife;
        MusicManager.Instance.ChangeToMusic();
    }

    private void TickBatteryLifetime()
    {
        if (CaptureQTEActive) return; //We don't want battery to drain during QTEs.

        if (IsDrainingFlashlight)
        {
            BatteryLifeRemaining = Mathf.Max(BatteryLifeRemaining - Time.deltaTime, 0);
            
            if (!IsEmptyLastFrame && BatteryLifeRemaining <= 0)
            {
                SfxFlashlightEmpty.Play();
                MusicManager.Instance.RanOutOfBattery();
                IsEmptyLastFrame = true;
            }

            if (!IsEmpty)
            {
                IsEmptyLastFrame = false;
            }
        }
        
        float ratio = BatteryLifeRemaining / MaxBatteryLife;
        
        BatteryLifeLowIndicator.SetActive(ratio <= 0.2f);

        bool empty = ratio <= 0;
        Flashlight.enabled = !empty;
        LightAround.enabled = !empty;
        LightStylish.enabled = !empty;
        
        Flashlight.pointLightInnerRadius = Mathf.Lerp(LightInnerOverLifetimeRatio.x, LightInnerOverLifetimeRatio.y, ratio);
        Flashlight.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio);
        
        LightAround.pointLightInnerRadius = 0;
        LightAround.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio * 0.5f);
        
        LightStylish.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, ratio);
    }

    public Vector2 GetAimDirection()
    {
        return AimDirection;
    }

    private void UpdateFlashlight()
    {
        //Calculations for aim direction and flashlight transform manipulation.
        Vector2 mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightPos = transform.position + Vector3.up*0.4f;
        Vector2 dir =  mousePos - flashlightPos;
        AimDirection = dir.normalized;
        
        float flipValue = dir.x < 0 ? -1 : 1;

        //Updates the transform of the player Spine skeleton's AIM bone if the magnitude of the aiming direction passes the given threshold.
        if (dir.magnitude > 0.6f)
        {
            SetFacingDirection(flipValue == 1);
            
            Flashlight.transform.up = dir;
            
            dir.x *= flipValue;
            Skeleton.skeleton.FindBone("AIM").SetLocalPosition(dir.normalized * 5);
        }
    }

    /// <summary>
    /// Override method that takes a specified direction as an argument instead of calculating it. Used for handling flashlight aim direction during ghost capture.
    /// </summary>
    /// <param name="overrideDirection"></param>
    private void UpdateFlashlight(Vector2 overrideDirection)
    {
        //Calculations for aim direction and flashlight transform manipulation.
        AimDirection = overrideDirection.normalized;

        float flipValue = overrideDirection.x < 0 ? -1 : 1;

        //Updates the transform of the player Spine skeleton's AIM bone if the magnitude of the aiming direction passes the given threshold.
        if (overrideDirection.magnitude > 0.6f)
        {
            SetFacingDirection(flipValue == 1);

            Flashlight.transform.up = overrideDirection;

            overrideDirection.x *= flipValue;
            Skeleton.skeleton.FindBone("AIM").SetLocalPosition(overrideDirection.normalized * 5);
        }
    }

    private void SetFacingDirection(bool faceRight)
    {
        Model.transform.localScale = new Vector3(faceRight ? 1 : -1, 1, 1);

        //Flashlight particles need to be adjusted for when scale's x is -1
        CaptureParticles.transform.localRotation = Quaternion.AngleAxis(faceRight ? 80 : 280, Vector3.forward);
    }

    private void DoCaptureInput()
    {
        bool prevCaptureActive = CaptureActive;
        CaptureActive = Input.GetMouseButton(0) && !IsEmpty;

        if (CaptureActive != prevCaptureActive)
        {
            Debug.Log("Changed vacuum state");
            ChangedVacuumState();
        }

        if (CaptureActive)
        {
            if (!GhostTarget)
            {
                GhostTarget = GhostCaptureHit();
            }
            else
            {
                //Only start the ghost capture QTE once the ghost reaches the pre-defined distance threshold.
                if (Vector2.Distance(transform.position, GhostTarget.transform.position) < CaptureDistanceForQTE)
                {
                    CaptureGhostQTE();
                }
            }
        }
        else
        {
            GhostTarget = null;
        }
    }

    /// <summary>
    /// Does a raycast out, and if the ghost is found in the cast checks if it's a valid capture target.
    /// </summary>
    /// <returns></returns>
    private GhostBase GhostCaptureHit()
    {
        var hits = Physics2D.RaycastAll(transform.position, AimDirection, CaptureCastLength);

        foreach (var hit in hits)
        {
            if (hit && hit.collider.gameObject.TryGetComponent<GhostBase>(out var ghost))
            {
                if (ghost && ghost.CanBeCaptured && !ghost.BlockCapture && !ghost.CheckForWalls(1000f))
                {
                    return ghost;
                }
            }
        }

        return null;
    }

    private void CaptureGhostQTE()
    {
        CaptureQTEActive = true;

        GhostTarget.StartCapture();
    }

    private void ChangedVacuumState()
    {
        VacuumTween?.Kill();

        if (CaptureActive)
        {
            SfxVacuumLoop.Play();


            VacuumTween = SfxVacuumLoop.DOFade(1, 0.2f);
        }
        else
        {
            VacuumTween = SfxVacuumLoop.DOFade(0, 0.2f);
        }
    }

    public void SetDialogueText(string content)
    {
        DialogueTween?.Complete(true);
        
        if (content != null)
        {
            DialogueText.text = content;
            DialogueTween = DialogueText.DOFade(1, 0.5f);
            return;
        }
        
        DialogueTween = DialogueText.DOFade(0, 0.2f);
    }

    //Cheats (Dev build only)
    private void TryCheats()
    {
        if (!Debug.isDebugBuild)
        {
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            //find and kill the nearest ghost
            var ghosts = FindObjectsByType<GhostBase>(FindObjectsSortMode.None);
            GhostBase nearestGhost = null;
            float nearestDistance = float.MaxValue;
            foreach (var ghost in ghosts)
            {
                float distance = Vector2.Distance(transform.position, ghost.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestGhost = ghost;
                }
            }
            if (nearestGhost != null)
            {
                Destroy(nearestGhost.gameObject);
            }
        }
    }
}
