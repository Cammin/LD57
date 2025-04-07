using System.Collections;
using CamLib;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class EnsureAllSoundsPlay : Singleton<EnsureAllSoundsPlay>
{
    public AudioResource[] AllSounds;
    public AudioSource Source;
    public int attempts = 20;
    
    public IEnumerator Start()
    {
        //#if UNITY_WEBGL
        
        for (int i = 0; i < attempts; i++)
        {
            foreach (AudioResource sound in AllSounds)
            {
                Source.resource = sound;
                Source.Play();
                yield return null;
            }
            yield return null;
        }
        
        //#endif
        
        //yield return new WaitForSeconds(1);
    }
}