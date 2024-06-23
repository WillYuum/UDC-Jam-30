using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTimeController : MonoBehaviour
{
    public enum DayNightState
    {
        Day,
        Night,
    }

    public DayNightState CurrentState { get; private set; } = DayNightState.Day;

    private float _dayDurationInMinutes = 6.0f;
    private float _nightDurationInMinutes = 5.5f;

    private GameTicker _gameTicker;

    [SerializeField] private Transform _sunTransform;
    [SerializeField] private Transform _moonTransform;

    void Awake()
    {
        _gameTicker = FindObjectOfType<GameTicker>();
    }



    void Update()
    {
        float gameTime = _gameTicker.GameTime;

        if (CurrentState == DayNightState.Day)
        {
            if (gameTime % _dayDurationInMinutes >= _dayDurationInMinutes * 60f)
            {
                ToggleState();
            }
        }
        else
        {
            if (gameTime % _dayDurationInMinutes >= _nightDurationInMinutes * 60f)
            {
                ToggleState();
            }
        }


        RotateVisuals(gameTime);

    }

    private void RotateVisuals(float gameTime)
    {
        Vector2 center = new Vector2(0f, 0f);
        float radius = 4.75f;
        float startPosition = Mathf.PI * 1.5f;
        _sunTransform.position = new Vector2(center.x + radius * Mathf.Cos(gameTime * 0.1f + startPosition), center.y + radius * -Mathf.Sin(gameTime * 0.1f + startPosition));
        _moonTransform.position = new Vector2(center.x + radius * Mathf.Cos(gameTime * 0.1f + startPosition + Mathf.PI), center.y + radius * -Mathf.Sin(gameTime * 0.1f + startPosition + Mathf.PI));

    }

    public void SetState(DayNightState state)
    {
        CurrentState = state;
    }

    public void ToggleState()
    {
        CurrentState = CurrentState == DayNightState.Day ? DayNightState.Night : DayNightState.Day;
        if (CurrentState == DayNightState.Day)
        {
            _sunTransform.gameObject.SetActive(true);
            _moonTransform.gameObject.SetActive(false);
        }
        else
        {
            _sunTransform.gameObject.SetActive(false);
            _moonTransform.gameObject.SetActive(true);
        }
    }
}
