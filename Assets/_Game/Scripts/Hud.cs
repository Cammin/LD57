using CamLib;
using UnityEngine;
using UnityEngine.UI;

public class Hud : Singleton<Hud>
{
    public const float SpaceBarEndDuration = .35f;

    public Image Heart1;
    public Image Heart2;
    public Image Heart3;
    [Space]
    public Image Bolt;
    public Outline BoltOutline;
    public Image BarsFill;
    public Gradient ColorOverBatteryLife;
    [Space]
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
        Player player = Player.Instance;
        if (player)
        {
            UpdateHearts(player);
            UpdateBatteryLife(player);
        }

        UpdateSpacebar();
    }

    private void UpdateBatteryLife(Player player)
    {
        float ratio = player.BatteryRatio;
        Color color = ColorOverBatteryLife.Evaluate(ratio);
        
        Bolt.color = color;
        BarsFill.color = color;
        BoltOutline.effectColor = color;
        
        BarsFill.fillAmount = ratio;
    }

    private void UpdateHearts(Player player)
    {
        Color goodColor = new Color(1, 0.1f, 0.1f, 1);
        Color badColor = new Color(1, 1, 1, 0.2f);
        if (player.GetHP() >= 3)
        {
            Heart1.color = goodColor;
            Heart2.color = goodColor;
            Heart3.color = goodColor;
        }
        else if (player.GetHP() == 2)
        {
            Heart1.color = goodColor;
            Heart2.color = goodColor;
            Heart3.color = badColor;
        }
        else if (player.GetHP() == 1)
        {
            Heart1.color = goodColor;
            Heart2.color = badColor;
            Heart3.color = badColor;
        }
        else
        {
            Heart1.color = badColor;
            Heart2.color = badColor;
            Heart3.color = badColor;
        }
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

