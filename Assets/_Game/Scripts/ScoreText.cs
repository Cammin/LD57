using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TMP_Text Txt;
    private void Start()
    {
        Txt.text = $"Total Score: {Player.Score}";
    }
}