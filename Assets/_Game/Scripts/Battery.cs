using UnityEngine;
using UnityEngine.Audio;

public class Battery : MonoBehaviour
{
    public int scoreOnCollect = 100;

    public Animator Anim;
    public bool collected;

    public AudioSource CollectSfx;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (collected) return;
            
            collected = true;
            player.RechargeBattery();
            player.AddScore(scoreOnCollect);
            
            Anim.SetTrigger("collect");
            
            
            CollectSfx.Play();
            
            GameManager.Instance.ImpulseColourVolume(Color.yellow);
            
            Destroy(gameObject, 1f);
            
            
        }
    }
}