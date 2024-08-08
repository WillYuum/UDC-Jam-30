using System;
using UnityEngine;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public enum DayNightState
    {
        Day,
        Night,
    }

    public class CurrentTimeInfo
    {
        public int Hour { get; private set; } = 8;
        public int Minute { get; private set; } = 0;
        public float Second { get; private set; } = 0;

        public string GetTimeInHoursAndMinutes()
        {
            return string.Format("{0:00}:{1:00}", Hour, Minute);
        }

        public string GetTimeInHoursAndMinutesAnSeconds()
        {
            return string.Format("{0:00}:{1:00}:{2:00}", Hour, Minute, (int)Second);
        }

        public void IncrementTimeInSeconds(float timeToIncrement)
        {
            Debug.Log("IncrementTime " + timeToIncrement);

            // Add the time increment to the current time
            float newTime = Second + timeToIncrement;
            Second = newTime % 60;  // Keep seconds within 0-59

            Minute += (int)newTime / 60;  // Add minutes if seconds go beyond 
            Hour += Minute / 60;          // Add hours if minutes go beyond 
            Minute %= 60;                 // Keep minutes within 0-

            Hour %= 24;                   // Keep hours within 0-23
        }
    }

    private CurrentTimeInfo _currentTimeInfo = new();
    public DayNightState CurrentState { get; private set; } = DayNightState.Day;

    public const float CycleDurationInRealTime = 30f; // Total duration of a full day-night cycle in seconds

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
        _currentDay = 1;
        CurrentState = DayNightState.Day; // Start with daytime
        timedEvents = new List<TimedEvent>();
        OnDayStart?.Invoke();
    }

    private readonly int _twentyFourHourInSecond = 86400;
    void Update()
    {
        _currentTimeInfo.IncrementTimeInSeconds(Time.deltaTime * _twentyFourHourInSecond / CycleDurationInRealTime);


        if (_currentTimeInfo.Hour == 8)
        {
            HandleChangeDayNightState(DayNightState.Day);
        }
        else if (_currentTimeInfo.Hour == 20)
        {
            HandleChangeDayNightState(DayNightState.Night);
        }
    }

    public void HandleChangeDayNightState(DayNightState state)
    {
        if (CurrentState == state)
        {
            return;
        }

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
        Debug.Log("New day started!!");
        foreach (var timedEvent in timedEvents)
        {
            if (timedEvent.dayNumber == _currentDay)
            {
                timedEvent.eventAction?.Invoke();
            }
        }
    }

    public DayNightState GetCurrentState()
    {
        return CurrentState;
    }

    public float GetDayTimeRatio()
    {
        if (CurrentState == DayNightState.Day)
        {
            return _currentTimeInfo.Hour / 12f;
        }
        return 0f; // No daytime ratio if it's night
    }

    public float GetNightTimeRatio()
    {
        if (CurrentState == DayNightState.Night)
        {
            return _currentTimeInfo.Hour / 12f;
        }
        return 0f; // No nighttime ratio if it's day
    }

    public string GetCurrentTimeIn24HourFormat()
    {
        return _currentTimeInfo.GetTimeInHoursAndMinutes();
    }
}
