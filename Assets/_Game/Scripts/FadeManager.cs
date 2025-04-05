using System;
using CamLib;
using DG.Tweening;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    public Image ScreenFade;

    private void Start()
    {
        FadeOut(Color.black);
    }

    [ContextMenu("FadeOut")]
    public void FadeOut(Color from)
    {
        ScreenFade.color = from;
        ScreenFade.DOColor(Color.clear, 1f);
    }
    
    [ContextMenu("FadeIn")]
    public void FadeIn() => FadeIn(Color.white, null);

    public void FadeIn(Color color, Action callback = null, float duration = 1, float delay = 0)
    {
        ScreenFade.color = Color.clear;
        ScreenFade.DOColor(color, duration)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                callback?.Invoke();
                FadeOut(color);
            });
    }
}