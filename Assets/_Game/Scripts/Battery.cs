using UnityEngine;

public class Battery : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.RechargeBattery();
            Destroy(gameObject);
        }
    }
}