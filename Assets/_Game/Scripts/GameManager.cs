using CamLib;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : Singleton<GameManager>
{
    public Light2D GlobalLight;
    public Volume DamageVolume;
    
    void Start()
    {
        
        
    }

    void Update()
    {
        
        
    }

    private Tween DamageTween;
    
    public void ImpulseDamageVolume()
    {
        DamageTween?.Complete(true);
        
        DamageVolume.weight = 1;
        
        DamageTween = DOVirtual.Float(DamageVolume.weight, 0, 1, value =>
        {
            DamageVolume.weight = value;
        });
    }
}
