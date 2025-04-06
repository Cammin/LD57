using UnityEngine;

public class Battery : MonoBehaviour
{
    public int scoreOnCollect = 100;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.RechargeBattery();
            player.AddScore(scoreOnCollect);
            Destroy(gameObject);
            
            
        }
    }
}