using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public GameObject Door;
    public AudioSource Sound;

    private void Start()
    {
        Door.SetActive(false);
    }

    public void OpenDoor()
    {
        Door.SetActive(true);
        Sound.Play();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            FadeManager.Instance.FadeIn(Color.white, LoadWinScene);
        }
    }

    private void LoadWinScene()
    {
        Checkpoint.ResetCheckpoint();
        SceneManager.LoadScene("MainMenu");
    }
}