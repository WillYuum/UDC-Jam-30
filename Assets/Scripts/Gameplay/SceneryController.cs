using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneryController : MonoBehaviour
{
    [SerializeField] private SceneryAssets _summerAssets;
    [SerializeField] private SceneryAssets _autumnAssets;
    [SerializeField] private SceneryAssets _winterAssets;


    [SerializeField] private SpriteRenderer _treeRender;
    [SerializeField] private SpriteRenderer _backgroundRender;

    void Start()
    {
        SeasonTimer seasonTimer = FindObjectOfType<SeasonTimer>();
        seasonTimer.OnSeasonChange += HandleSeasonChange;
    }



    private void HandleSeasonChange(SeasonTimer.Season season)
    {
        SceneryAssets sceneryAssets = null;

        switch (season)
        {
            case SeasonTimer.Season.Summer:
                sceneryAssets = _summerAssets;
                break;
            case SeasonTimer.Season.Autumn:
                sceneryAssets = _autumnAssets;
                break;
            case SeasonTimer.Season.Winter:
                sceneryAssets = _winterAssets;
                break;
        }

        _treeRender.sprite = sceneryAssets.TreeVisual;
        _backgroundRender.sprite = sceneryAssets.BackgroundVisual;
    }


}



[System.Serializable]
class SceneryAssets
{
    [field: SerializeField] public Sprite TreeVisual { get; private set; }
    [field: SerializeField] public Sprite BackgroundVisual { get; private set; }
}



