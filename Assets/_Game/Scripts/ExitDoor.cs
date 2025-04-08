using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public GameObject Door;
    public AudioSource Sound;

    public bool Revealed;
    
    private void Start()
    {
        Door.SetActive(false);
    }

    public void OpenDoor()
    {
        Door.SetActive(true);
        Sound.Play();
        Revealed = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Revealed) return;
        
        if (other.TryGetComponent<Player>(out var player))
        {
            player.InvincibilityTime = 5;
            FadeManager.Instance.FadeIn(Color.white, LoadWinScene);
        }
    }

    private void LoadWinScene()
    {
        if (MusicManager.HasInstance)
        {
            Destroy(MusicManager.Instance.gameObject);
        }
        
        Checkpoint.ResetCheckpoint();
        IntroOutro.IsOutro = true;
        SceneManager.LoadScene("IntroOutro");
    }
}