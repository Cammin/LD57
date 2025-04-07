using CamLib;
using DG.Tweening;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    public AudioSource Ambience;
    public AudioSource Music;
    
    private void Start()
    {
        Ambience.Play();
        Music.Play();
        Music.Pause();
    }
    
    public void ChangeToMusic()
    {
        if (Music.isPlaying)
        {
            return;
        }
        Music.UnPause();

        if (!Ambience.isPlaying)
        {
            Ambience.Play();
        }
        Ambience.DOFade(0.05f, 1);
    }

    public void RanOutOfBattery()
    {
        Music.Pause();
    }

}