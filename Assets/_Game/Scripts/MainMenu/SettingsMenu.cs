using CamLib;
using UnityEngine;

public class SettingsMenu : Singleton<SettingsMenu>
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
}