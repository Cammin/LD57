using LDtkUnity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomLevel : MonoBehaviour, ILDtkImportedFields, ILDtkImportedLevel
{
    public bool OverrideLight = false;
    public float OverrideLightStrength = 1f;
    public int levelIndex;
    public const int maxLevel = 20;
    
    public Volume Volume;
    public BoxCollider LightingArea;
    public Light2D GlobalLight => GameManager.Instance.GlobalLight;

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
        float factor = (levelIndex / (float)maxLevel);
        if (OverrideLight)
        {
            factor = OverrideLightStrength;
        }
        factor = 1-factor;
        
        float lightValue = Mathf.Lerp(0.01f, 0.1f, factor);
        
        
        
        Debug.Log($"set to! levelIndex: {levelIndex}, factor:{factor}, lightValue:{lightValue}");
        GlobalLight.intensity = lightValue;
        Volume.weight = lightValue;
    }

    public void OnLDtkImportLevel(Level level)
    {
        Rect rect = level.UnityWorldSpaceBounds(WorldLayout.Free, 28);
        
        LightingArea.size = new Vector2(rect.width, rect.height);
        LightingArea.center = LightingArea.size* 0.5f;
    }

    public void OnLDtkImportFields(LDtkFields fields)
    {
        levelIndex = fields.GetInt("LevelOrder");
        
        OverrideLight = !fields.IsNull("LightStrength");

        if (OverrideLight)
        {
            OverrideLightStrength = fields.GetFloat("LightStrength");
        }
        
    }
}