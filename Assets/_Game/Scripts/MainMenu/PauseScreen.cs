using CamLib;
using UnityEngine;

public class PauseScreen : Singleton<PauseScreen>
{
    public GameObject Menu;
    public bool Paused;
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
            
            Time.timeScale = Paused ? 0 : 1;
            
            if (Paused)
            {
                Menu.SetActive(true);
            }
            else
            {
                Menu.SetActive(false);
            }
        }
    }
}