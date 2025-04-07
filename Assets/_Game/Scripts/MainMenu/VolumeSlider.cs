using CamLib;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider Slider;
    public AudioMixerGroup Group;
    public string PrefKey = "Volume";
    public float DefaultInitialVolume = 0.5f;

    private void Start()
    {
        Slider.onValueChanged.AddListener(value =>
        {
            PlayerPrefs.SetFloat(PrefKey, value);
            Group.SetNormalizedVolume(value);
        });
            
        Slider.value = PlayerPrefs.GetFloat(PrefKey, DefaultInitialVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}