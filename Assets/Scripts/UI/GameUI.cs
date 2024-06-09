using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI GameTimeText;
    [SerializeField] public LevelIndicators LevelIndicators;
    [SerializeField] public IntroScreen IntroScreen;

    public void PlayGameIntro(Action onEnd)
    {
        IntroScreen.FadeInScreen(onEnd);
    }
}
