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
    [SerializeField] private float _summerDurationInSeconds = 90f;
    [SerializeField] private float _autumnDurationInSeconds = 180f;
    [SerializeField] private float _winterDurationInSeconds = 220f;

    public event Action<Season> OnSeasonChange;

    private GameTicker _gameTicker;


    public void StartSeasonTimer()
    {
        _timeTillNextSeason = _autumnDurationInSeconds;
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
        if (_gameTicker.IsFastMode)
        {
            // When in fast mode, adjust season timer based on fast forward multiplier
            _timeTillNextSeason -= _gameTicker.FastForwardMultiplier * _gameTicker.TickDuration;
        }
        else
        {
            // Otherwise, decrement based on regular tick duration
            _timeTillNextSeason -= _gameTicker.TickDuration;
        }

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


public class SeasonData
{
    public SeasonTimer.Season season;
    public float durationInSeconds;

    public SeasonData(SeasonTimer.Season season, float durationInSeconds)
    {
        this.season = season;
        this.durationInSeconds = durationInSeconds;
    }
}