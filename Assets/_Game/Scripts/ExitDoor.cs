using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public GameObject Door;

    private void Start()
    {
        Door.SetActive(false);
    }

    public void OpenDoor()
    {
        Door.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            Hud.Instance.FadeIn(Color.white, LoadWinScene);
        }
    }

    private void LoadWinScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}