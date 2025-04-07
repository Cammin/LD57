using CamLib;
using UnityEngine;
using UnityEngine.UI;

public class Hud : Singleton<Hud>
{
    public const float SpaceBarEndDuration = .35f;

    public GameObject SpaceBar;

    public GameObject[] SpaceBarUp;
    public GameObject[] SpaceBarDown;
    public Image[] SpaceBarGreen;

    private bool SpaceBarImageUp = true;
    private float SpaceBarElapsed;

    private void Start()
    {
        SpaceBar.SetActive(false);
    }

    private void Update()
    {
        UpdateSpacebar();
    }

    public void ShowSpacebar(bool show)
    {
        SpaceBar.SetActive(show);

        if (show)
        {
            SpaceBarImageUp = true;
            SpaceBarElapsed = 0f;

            foreach (var green in SpaceBarGreen)
            {
                green.fillAmount = 0f;
            }
        }
    }

    private void UpdateSpacebar()
    {
        if (SpaceBar.activeSelf)
        {
            if (SpaceBarElapsed >= SpaceBarEndDuration)
            {
                SpaceBarImageUp = !SpaceBarImageUp;
                SpaceBarElapsed = 0f;
            }
            else
            {
                SpaceBarElapsed += Time.deltaTime;
            }

            foreach (var up in SpaceBarUp)
            {
                up.SetActive(SpaceBarImageUp);
            }

            foreach (var down in SpaceBarDown)
            {
                down.SetActive(!SpaceBarImageUp);
            }

            if (Player.Instance.GhostTarget)
            {
                foreach (var green in SpaceBarGreen)
                {
                    green.fillAmount = Player.Instance.GhostTarget.CaptureProgress / 100f;
                }
            }
        }
    }
}

