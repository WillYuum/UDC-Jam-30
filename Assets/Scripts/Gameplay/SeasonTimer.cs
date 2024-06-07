using System;
using UnityEngine;

public class SeasonTimer : MonoBehaviour
{
    public enum Season
    {
        Summer,
        Autumn,
        Winter
    }

    public Season CurrentSeason { get; private set; } = Season.Summer;

    private float _timeTillNextSeason;
    [SerializeField] private float _summerDurationInSeconds = 90f; // 1.5 minutes
    [SerializeField] private float _autumnDurationInSeconds = 60f; // 1 minute
    [SerializeField] private float _winterDurationInSeconds = 60f; // 1 minute

    public event Action<Season> OnSeasonChange;

    private GameTicker _gameTicker;

    void Start()
    {
        _timeTillNextSeason = _summerDurationInSeconds;
        _gameTicker = FindObjectOfType<GameTicker>();

        if (_gameTicker != null)
        {
            _gameTicker.OnTick += HandleTick;
        }
        else
        {
            Debug.LogError("GameTicker not found in the scene.");
        }
    }

    private void HandleTick()
    {
        // Adjust the decrement based on whether the game ticker is in fast mode
        float tickDuration = _gameTicker.IsFastMode ? _gameTicker.FastForwardMultiplier : 1f;
        _timeTillNextSeason -= tickDuration;

        if (_timeTillNextSeason <= 0)
        {
            SwitchToNextSeason();
        }
    }

    private void SwitchToNextSeason()
    {
        switch (CurrentSeason)
        {
            case Season.Summer:
                CurrentSeason = Season.Autumn;
                _timeTillNextSeason = _autumnDurationInSeconds;
                break;
            case Season.Autumn:
                CurrentSeason = Season.Winter;
                _timeTillNextSeason = _winterDurationInSeconds;
                break;
            case Season.Winter:
                CurrentSeason = Season.Summer;
                _timeTillNextSeason = _summerDurationInSeconds;
                break;
        }

        OnSeasonChange?.Invoke(CurrentSeason);
    }

    private void OnDestroy()
    {
        if (_gameTicker != null)
        {
            _gameTicker.OnTick -= HandleTick;
        }
    }
}
