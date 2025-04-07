using CamLib;
using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : Singleton<Player>
{
    public int HP;

    public float moveSpeed;
    public Rigidbody2D _rb;
    public Light2D flashlight;
    public Light2D lightAround;
    public Light2D lightStylish;

    private Vector2 MoveInput;

    public float MaxBatteryLife;
    private float BatteryLifeRemaining;
    public GameObject BatteryLifeLowIndicator;
    

    public Vector2 LightInnerOverLifetimeRatio;
    public Vector2 LightOuterOverLifetimeRatio;

    public float InvincibilityTime;
    
    public GameObject Model;

    public CinemachineImpulseSource Impulse;

    public Animator Anim;
    public SkeletonRenderer Skeleton;

    public AudioSource SfxFootstep;
    public AudioSource SfxHurt;
    public AudioSource SfxImpact;
    public AudioSource SfxDeath;
    public AudioSource SfxFlashlightEmpty;
    public AudioSource SfxQteMash;
    public AudioSource SfxVacuumLoop;
    
    public bool IsEmpty => BatteryLifeRemaining <= 0;
    public bool IsEmptyLastFrame;

    public float ProgressPerQTEHit = 15f;

    public ParticleSystem CaptureParticles;

    public Transform PulseTextSpawnPoint;

    public const float CaptureCastRadius = 1f;
    public const float CaptureCastLength = 10f;
    public const float CaptureDistanceForQTE = 4f;

    private Vector2 AimDirection;

    [NonSerialized] public GhostBase GhostTarget;
    [NonSerialized] public bool CaptureActive;
    [NonSerialized] public bool CaptureQTEActive;
    
    public bool IsDead => HP <= 0;

    public float BatteryRatio => BatteryLifeRemaining / MaxBatteryLife;

    public bool IsDrainingFlashlight;

    private void Start()
    {
        _camera = Camera.main;
        BatteryLifeRemaining = MaxBatteryLife;

        DialogueText.color = new Color(1, 1, 1, 0);

        //SfxVacuumLoop.volume = 0;

        
    }

    private bool prevParticlesState;
    
    private void Update()
    {
        if (Time.timeScale <= 0) return;
        
        Anim.SetBool("Flashlight", !IsEmpty);
        
        if (IsDead) return;

        bool newParticlesState = CaptureActive || CaptureQTEActive;
        if (newParticlesState != prevParticlesState)
        {
            prevParticlesState = newParticlesState;
            if (newParticlesState)
            {
                CaptureParticles.Play();
            }
            else
            {
                CaptureParticles.Stop();
            }
        }
        

        flashlight.color = CaptureActive ? Color.cyan : Color.white;

        DoMoveInput();
        TryCheats();

        //If in the middle of a QTE we don't want the rest of the update to happen.
        if (CaptureQTEActive)
        {
            if (GhostTarget)
            {
                UpdateFlashlight(GhostTarget.transform.position - transform.position);

                if (!GhostTarget.IsBeingDestroyed() && Input.GetKeyDown(KeyCode.Space))
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

    private Tween VacuumTween;
    
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

    private GhostBase GhostCaptureHit()
    {
        var hits = Physics2D.RaycastAll(transform.position, AimDirection, CaptureCastLength);

        foreach(var hit in hits)
        {
            if (hit && hit.collider.gameObject.TryGetComponent<GhostBase>(out var ghost))
            {
                //Debug.Log("hit ghost");
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

    private void TickBatteryLifetime()
    {
        if (CaptureQTEActive) return;

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
        flashlight.enabled = !empty;
        lightAround.enabled = !empty;
        lightStylish.enabled = !empty;
        
        flashlight.pointLightInnerRadius = Mathf.Lerp(LightInnerOverLifetimeRatio.x, LightInnerOverLifetimeRatio.y, ratio);
        flashlight.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio);
        
        
        lightAround.pointLightInnerRadius = 0;
        lightAround.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio * 0.5f);
        
        lightStylish.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, ratio);
    }

    private void DoMoveInput()
    {
        //translate player
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horiz, vert, 0).normalized;
        MoveInput = move;

        Anim.SetBool("Moving", MoveInput.magnitude > 0);
    }

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

    private void UpdateFlashlight()
    {
        //direction to mouse
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightPos = transform.position + Vector3.up*0.4f;
        Vector2 dir =  mousePos - flashlightPos;
        AimDirection = dir.normalized;
        
        float flipValue = dir.x < 0 ? -1 : 1;

        //AIM BONE
        if (dir.magnitude > 0.6f)
        {
            SetFacingDirection(flipValue == 1);
            
            flashlight.transform.up = dir;
            
            dir.x *= flipValue;
            Skeleton.skeleton.FindBone("AIM").SetLocalPosition(dir.normalized * 5);
        }
    }

    public Vector2 GetAimDirection()
    {
        return AimDirection;
    }

    private void UpdateFlashlight(Vector2 overrideDirection)
    {
        //direction to mouse
        AimDirection = overrideDirection.normalized;

        float flipValue = overrideDirection.x < 0 ? -1 : 1;

        //AIM BONE
        if (overrideDirection.magnitude > 0.6f)
        {
            SetFacingDirection(flipValue == 1);

            flashlight.transform.up = overrideDirection;

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

    private void FixedUpdate()
    {
        _rb.linearVelocity = MoveInput * (moveSpeed);
    }

    public bool TryTakeDamage()
    {
        if (IsDead) return false;
        
        if (InvincibilityTime > 0) return false;
        InvincibilityTime = 0.5f;
        
        HP--;
        Impulse.GenerateImpulse();
        GameManager.Instance.ImpulseColourVolume(Color.red);
        
        SfxImpact.Play();
        
        if (HP <= 0)
        {
            Debug.Log("Player is dead");
            Anim.SetTrigger("death");
            MoveInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
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

    public void RechargeBattery()
    {
        BatteryLifeRemaining = MaxBatteryLife;
        MusicManager.Instance.ChangeToMusic();
    }
    
    public void Heal()
    {
        HP = 3;
    }

    public static int Score;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetScore()
    {
        Score = 0;
    }
    
    public void AddScore(int score)
    {
        GameManager.Instance.CreatePulseText($"+{score} score", Color.white);

        Score += score;
        //update UI etc
    }

    public TMP_Text DialogueText;

    private Tween DialogueTween;
    public Camera _camera;

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
}
