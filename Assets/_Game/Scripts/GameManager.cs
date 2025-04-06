using CamLib;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : Singleton<GameManager>
{
    public Light2D GlobalLight;
    public Volume ColourVolume;
    
    void Start()
    {
        
        
    }

    void Update()
    {
        
        
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
}
