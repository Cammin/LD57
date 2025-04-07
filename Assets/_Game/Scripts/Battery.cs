using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

public class Battery : MonoBehaviour
{
    public int scoreOnCollect = 100;

    public Animator Anim;
    public bool collected;

    public AudioSource CollectSfx;
    
    public Light2D TurnOffLightUponCollect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>(other);
        if (player)
        {
            if (collected) return;
            
            collected = true;
            player.RechargeBattery();
            player.AddScore(scoreOnCollect);
            
            Anim.SetTrigger("collect");

            TurnOffLightUponCollect.enabled = false;
            CollectSfx.Play();
            
            GameManager.Instance.ImpulseColourVolume(Color.yellow);
            
            Destroy(gameObject, 2f);
            
            
        }
    }
}