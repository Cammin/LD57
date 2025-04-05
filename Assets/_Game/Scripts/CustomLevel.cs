using LDtkUnity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomLevel : MonoBehaviour, ILDtkImportedFields, ILDtkImportedLevel
{
    public float LightStrength = 1f;
    public Volume Volume;
    public BoxCollider LightingArea;
    public Light2D GlobalLight;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            SetVolumeAmountFlashColor();
        }
    }

    public void SetVolumeAmountFlashColor()
    {
        Debug.Log($"set to! {LightStrength}");
        GlobalLight = GameManager.Instance.GlobalLight;
        GlobalLight.intensity = LightStrength;
        Volume.weight = LightStrength;
    }
    
    

    public void OnLDtkImportLevel(Level level)
    {
        Rect rect = level.UnityWorldSpaceBounds(WorldLayout.Free, 28);
        
        LightingArea.size = new Vector2(rect.width, rect.height);
        LightingArea.center = LightingArea.size* 0.5f;
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        LightStrength = fields.GetFloat("LightStrength");
        
    }
}