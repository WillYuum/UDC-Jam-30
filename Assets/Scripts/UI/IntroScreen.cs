using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;
public class IntroScreen : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI[] _allTexts;
    [SerializeField] private Image _backgroundImage;


    void Awake()
    {
        _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        foreach (TextMeshProUGUI text in _allTexts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }
    }


    public void FadeInScreen(Action OnEnd)
    {
        _backgroundImage.DOFade(1, 1).SetDelay(0.75f);

        //sequentially fade in all texts with delay of 1.5 seconds
        for (int i = 0; i < _allTexts.Length; i++)
        {

            if (i == _allTexts.Length - 1)
            {
                _allTexts[i].DOFade(1, 1).SetDelay(1.5f * i).OnComplete(() => HideScreen(OnEnd));
            }
            else
            {
                _allTexts[i].DOFade(1, 1).SetDelay(1.5f * i);
            }
        }
    }


    private void HideScreen(Action onEnd)
    {
        foreach (TextMeshProUGUI text in _allTexts)
        {
            text.DOFade(0, 1);
        }

        _backgroundImage.DOFade(0, 1).SetDelay(1.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onEnd();
        });
    }
}
