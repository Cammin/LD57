using DevLocker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroOutro : MonoBehaviour
{
    public static bool IsOutro;


    public SceneReference ToGameplay;
    public SceneReference ToTitleScreen;
    
    
    public float TimeUntilCanClick = 5;
    public float Timer;

    public GameObject ClickToContinue;

    public GameObject IntroImage;
    public GameObject OutroImage;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetState()
    {
        IsOutro = false;
    }

    public AudioSource IntroMusic;
    public AudioSource OutroMusic;
    
    public AudioSource SourceToPlay => IsOutro ? OutroMusic : IntroMusic;

    private AsyncOperation async;

    private void Start()
    {
        SourceToPlay.Play();
        
        IntroImage.SetActive(!IsOutro);
        OutroImage.SetActive(IsOutro);
        
        string destScene = IsOutro ? ToTitleScreen.ScenePath : ToGameplay.ScenePath;
        async = SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Single);
        async.allowSceneActivation = false;
    }
    
    private void Update()
    {
        Timer += Time.deltaTime;
        
        bool canClick = Timer > TimeUntilCanClick;
        ClickToContinue.SetActive(canClick);

        if (canClick && Input.GetMouseButtonDown(0))
        {
            
            Player.ResetScore();
            IsOutro = false;
            FadeManager.Instance.FadeIn(Color.black, LoadNextScene);

            void LoadNextScene()
            {
                async.allowSceneActivation = true;
            }
        }
    }
}