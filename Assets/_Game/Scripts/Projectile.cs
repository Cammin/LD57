using UnityEngine;

public class Projectile : MonoBehaviour
{
    //shot by the enemy

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage();
            Destroy(gameObject);
        }
    }
}
