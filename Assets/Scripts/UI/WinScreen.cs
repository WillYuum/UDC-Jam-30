using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    [SerializeField] private TextMeshProUGUI[] _allTexts;

    private bool _isPlaying = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && _isPlaying)
        {
            GameManager.instance.RestartGame();
        }
    }

    public void PlayWinScreen()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().DOFade(1, 1).SetDelay(0.75f);

        _isPlaying = true;

        //sequentially fade in all texts with delay of 1.5 seconds
        for (int i = 0; i < _allTexts.Length; i++)
        {

            if (i == _allTexts.Length - 1)
            {
                _allTexts[i].DOFade(1, 1).SetDelay(1.5f * i).OnComplete(() =>
                {
                    _isPlaying = false;
                });
            }
            else
            {
                _allTexts[i].DOFade(1, 1).SetDelay(1.5f * i);
            }
        }
    }
}
