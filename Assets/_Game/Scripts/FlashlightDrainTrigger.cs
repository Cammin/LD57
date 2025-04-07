using UnityEngine;

public class FlashlightDrainTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.IsDrainingFlashlight = true;

            MusicManager.Instance.ChangeToMusic();
        }
    }
}