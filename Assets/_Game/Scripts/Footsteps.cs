using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource FootstepsAudio;
        
    public void footstep()
    {
        FootstepsAudio.Play();
    }
}