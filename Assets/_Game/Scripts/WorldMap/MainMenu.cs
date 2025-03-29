using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsObject;
    public SceneReference GameplayScene;
    
    public GameObject QuitButton;

    private void Start()
    {
        //if platform is webgl, hide the quit button
#if UNITY_WEBGL
        QuitButton.gameObject.SetActive(false);
#endif
    }

    public void OnPlayButtonPressed()
    {
        SceneManager.LoadScene(GameplayScene.ScenePath);
    }

    public void OnOptionsButtonPressed()
    {
        SettingsObject.SetActive(true);
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();   
#else
        Application.Quit();
#endif
    }
}
