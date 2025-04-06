using CamLib;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : Singleton<Player>
{
    public int HP;

    public float moveSpeed;
    public Rigidbody2D _rb;
    public Light2D flashlight;
    public Light2D lightAround;

    private Vector2 MoveInput;

    public float MaxBatteryLife;
    private float BatteryLifeRemaining;
    public GameObject BatteryLifeLowIndicator;

    public Vector2 LightInnerOverLifetimeRatio;
    public Vector2 LightOuterOverLifetimeRatio;

    public float InvincibilityTime;
    
    public GameObject Model;

    public CinemachineImpulseSource Impulse;
    
    public bool IsEmpty => BatteryLifeRemaining <= 0;

    public float CaptureForceModifier = 5f;

    public const float CaptureCastRadius = 1f;
    public const float CaptureCastLength = 5f;
    public const float DefaultCaptureForce = 1000f;

    private Vector2 AimDirection => (_camera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

    [NonSerialized] private GhostBase GhostTarget;
    [NonSerialized] private bool CaptureActive;
    
    private void Start()
    {
        _camera = Camera.main;
        BatteryLifeRemaining = MaxBatteryLife;
        
        DialogueText.color = Color.clear;
    }

    private void Update()
    {
        DoMoveInput();
        TryCheats();
        UpdateFlashlight();
        
        TickBatteryLifetime();
        
        InvincibilityTime -= Time.deltaTime;

        CaptureActive = Input.GetMouseButton(0);
    }

    private GhostBase GhostCaptureHit()
    {
        var hit = Physics2D.CircleCast(transform.position, CaptureCastRadius, AimDirection, CaptureCastLength);

        if (hit && hit.collider.gameObject.TryGetComponent<GhostBase>(out var ghost))
        {
            if (ghost && ghost.CanBeCaptured && !ghost.BlockCapture)
            {
                return ghost;
            }
        }

        return null;
    }

    private void CaptureGhost(GhostBase ghost)
    {
        ghost.BlockActions = true;
        ghost.BlockCapture = true;
        ghost.BlockMovement = true;

        //TODO animate

        StartCoroutine(CoCapture());

        IEnumerator CoCapture()
        {
            yield return new WaitForSeconds(1); //TODO change to anim time

            AddScore(ghost.ScoreAddedForCapture);
            Destroy(ghost.gameObject);
        }
    }

    private void TickBatteryLifetime()
    {
        BatteryLifeRemaining = Mathf.Max(BatteryLifeRemaining - Time.deltaTime, 0);
        
        float ratio = BatteryLifeRemaining / MaxBatteryLife;
        
        BatteryLifeLowIndicator.SetActive(ratio <= 0.2f);

        bool empty = ratio <= 0;
        flashlight.enabled = !empty;
        lightAround.enabled = !empty;
        
        flashlight.pointLightInnerRadius = Mathf.Lerp(LightInnerOverLifetimeRatio.x, LightInnerOverLifetimeRatio.y, ratio);
        flashlight.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio);
        
        
        lightAround.pointLightInnerRadius = 0;
        lightAround.pointLightOuterRadius = Mathf.Lerp(LightOuterOverLifetimeRatio.x, LightOuterOverLifetimeRatio.y, ratio * 0.5f);
    }

    private void DoMoveInput()
    {
        //translate player
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horiz, vert, 0).normalized;
        MoveInput = move;
        _rb.linearVelocity = MoveInput * (moveSpeed);
    }

    private void TryCheats()
    {
        if (!Debug.isDebugBuild)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(2))
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        flashlight.transform.up = AimDirection;
        
        
        //flip player model
        Vector3 scale = Model.transform.localScale;
        float flipValue = AimDirection.x < 0 ? -1 : 1;
        scale.x = Mathf.Abs(scale.x) * flipValue;
        Model.transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        if (CaptureActive)
        {
            if (!GhostTarget)
            {
                GhostTarget = GhostCaptureHit();
                GhostTarget.CaptureInProgress = true;
            }
            else
            {
                var direction = transform.position - GhostTarget.transform.position;
                GhostTarget.Rigidbody.AddForce(direction * CaptureForceModifier * DefaultCaptureForce * Time.deltaTime);

                if (Vector2.Distance(transform.position, GhostTarget.transform.position) < .25f)
                {
                    CaptureGhost(GhostTarget);
                    GhostTarget = null;
                }
            }
        }
    }

    public bool TryTakeDamage()
    {
        if (InvincibilityTime > 0) return false;
        InvincibilityTime = 0.5f;
        
        HP--;
        Impulse.GenerateImpulse();
        GameManager.Instance.ImpulseDamageVolume();
        
        if (HP <= 0)
        {
            Debug.Log("Player is dead");
            
            FadeManager.Instance.FadeIn(Color.black, () =>
            {
                SceneManager.LoadScene("Gameplay");
            });
            
            
        }
        else
        {
            // Handle player taking damage
            Debug.Log("Player took damage, remaining HP: " + HP);
            
        }

        return true;
    }

    public void RechargeBattery()
    {
        BatteryLifeRemaining = MaxBatteryLife;
    }
    
    public void Heal()
    {
        HP = 3;
    }

    public int Score;
    
    public void AddScore(int score)
    {
        Score += score;
        Debug.Log("Score: " + Score);
        //update UI etc
    }

    public TMP_Text DialogueText;

    private Tween DialogueTween;
    private Camera _camera;

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
