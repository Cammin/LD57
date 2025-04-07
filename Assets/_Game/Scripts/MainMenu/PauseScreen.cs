using CamLib;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : Singleton<PauseScreen>
{
    public GameObject Menu;
    public bool Paused;

    public SettingsMenu Settings;

    private void Start()
    {
        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0 : 1;
        
        if (Paused)
        {
            Menu.SetActive(true);
        }
        else
        {
            Close();
        }
    }

    public void Close()
    {
        Time.timeScale = 1;
        Paused = false;
        Menu.SetActive(false);
        Settings.gameObject.SetActive(false);
    }
    

    public void QuitToTitleScreen()
    {
        Debug.Log("quit");
        Checkpoint.ResetCheckpoint();
        
        FadeManager.Instance.FadeIn(Color.black, Callback, 1);

        void Callback()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}