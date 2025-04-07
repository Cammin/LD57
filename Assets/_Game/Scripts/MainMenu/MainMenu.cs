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
        
        
        //destroy music manager if it exists
        if (MusicManager.HasInstance)
        {
            Destroy(MusicManager.Instance.gameObject);
        }
    }

    public void OnPlayButtonPressed()
    {
        FadeManager.Instance.FadeIn(Color.black, () =>
        {
            SceneManager.LoadScene(GameplayScene.ScenePath);
        }, 1f);
        
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
