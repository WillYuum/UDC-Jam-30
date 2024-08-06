using UnityEngine;
using System;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public enum DayNightState
    {
        Day,
        Night,
    }

    public DayNightState CurrentState { get; private set; } = DayNightState.Day;

    [HideInInspector] public float DayDurationInSeconds { get; private set; } = 7f;
    [HideInInspector] public float NightDurationInSeconds { get; private set; } = 7f;
    private float _currentTime;
    private int _currentDay;
    public event Action OnDayStart;
    public event Action OnNightStart;

    public List<TimedEvent> timedEvents;

    public struct TimedEvent
    {
        public int dayNumber;
        public Action eventAction;
    }

    void Start()
    {
        _currentTime = 0f;
        _currentDay = 1;
        CurrentState = DayNightState.Day; // Start with day time
        timedEvents = new List<TimedEvent>();
        OnDayStart?.Invoke();
    }

    void Update()
    {
        _currentTime += Time.deltaTime;

        if ((CurrentState == DayNightState.Day && _currentTime >= DayDurationInSeconds) ||
            (CurrentState == DayNightState.Night && _currentTime >= NightDurationInSeconds))
        {
            _currentTime = 0f;
            HandleChangeDayNightState(CurrentState == DayNightState.Day ? DayNightState.Night : DayNightState.Day);
        }
    }

    public void HandleChangeDayNightState(DayNightState state)
    {
        CurrentState = state;
        if (CurrentState == DayNightState.Day)
        {
            _currentDay++;
            OnDayStart?.Invoke();
            HandleDayEvent();
        }
        else
        {
            OnNightStart?.Invoke();
        }
    }

    private void HandleDayEvent()
    {
        foreach (var timedEvent in timedEvents)
        {
            if (timedEvent.dayNumber == _currentDay)
            {
                timedEvent.eventAction?.Invoke();
            }
        }
    }

    public float GetCurrentTime()
    {
        return _currentTime;
    }
}
