using CamLib;
using UnityEngine;
using UnityEngine.UI;

public class Hud : Singleton<Hud>
{
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    public Image Bolt;
    public Outline BoltOutline;
    public Image BarsFill;
    public Gradient ColorOverBatteryLife;
    
    private void Update()
    {
        Player player = Player.Instance;
        if (player)
        {
            UpdateHearts(player);
            UpdateBatteryLife(player);
        }
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
        if (player.HP >= 3)
        {
            Heart1.color = goodColor;
            Heart2.color = goodColor;
            Heart3.color = goodColor;
        }
        else if (player.HP == 2)
        {
            Heart1.color = goodColor;
            Heart2.color = goodColor;
            Heart3.color = new Color(1, 1, 1, 0.2f);
        }
        else if (player.HP == 1)
        {
            Heart1.color = goodColor;
            Heart2.color = new Color(1, 1, 1, 0.2f);
            Heart3.color = new Color(1, 1, 1, 0.2f);
        }
        else
        {
            Heart1.color = new Color(1, 1, 1, 0.2f);
            Heart2.color = new Color(1, 1, 1, 0.2f);
            Heart3.color = new Color(1, 1, 1, 0.2f);
        }
    }
}

