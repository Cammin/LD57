using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class EnsureAllSoundsPlay : MonoBehaviour
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
                yield return new WaitForSeconds(0.01f);
            }
        
            yield return new WaitForSeconds(0.1f);
        }
        
        //#endif
        
        //yield return new WaitForSeconds(1);
    }
}