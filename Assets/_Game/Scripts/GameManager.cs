using CamLib;
using DG.Tweening;
using System;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : Singleton<GameManager>
{
    public const float DefaultZoom = 5.5f;

    public CinemachineCamera Camera;
    public Light2D GlobalLight;
    public Volume ColourVolume;

    [NonSerialized] public Transform OverrideCameraTarget;
    private Tween CurrentZoomTween;

    private void Start()
    {
        
    }

    void Update()
    {
        Camera.Follow = OverrideCameraTarget != null ? OverrideCameraTarget : Player.Instance.transform;
    }

    private Tween ColourTween;
    
    public void ImpulseColourVolume(Color colour)
    {
        ColourVolume.profile.TryGet<Bloom>(out var bloom);
        bloom.tint.value = colour;

        ColourTween?.Complete(true);
        
        ColourVolume.weight = 1;

        ColourTween = DOVirtual.Float(ColourVolume.weight, 0, 1, value =>
        {
            ColourVolume.weight = value;
        });
    }

    public void CameraZoom(float zoom)
    {
        // Kill existing tween if it's running
        if (CurrentZoomTween != null && CurrentZoomTween.IsActive())
        {
            CurrentZoomTween.Kill();
        }

        CurrentZoomTween = DOTween.To(
            () => Camera.Lens.OrthographicSize,
            x => Camera.Lens.OrthographicSize = x,
            zoom,
            .2f
        ).SetEase(Ease.InOutSine);
    }

    public void CameraResetZoom()
    {
        CameraZoom(DefaultZoom);
    }
}
