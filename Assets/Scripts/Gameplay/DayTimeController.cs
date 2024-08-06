using System;
using UnityEngine;

public class DayTimeController : MonoBehaviour
{
    [SerializeField] private Transform _sunTransform;
    [SerializeField] private Transform _moonTransform;

    private float _moonSunDistFromCenter = 4.75f;
    private TimeManager _timeManager;

    private void Awake()
    {
        _timeManager = FindObjectOfType<TimeManager>();
        if (_timeManager == null)
        {
            Debug.LogError("TimeManager not found in the scene.");
            enabled = false;
            return;
        }

        _timeManager.OnDayStart += () => HandleChangeDayNightState(TimeManager.DayNightState.Day);
        _timeManager.OnNightStart += () => HandleChangeDayNightState(TimeManager.DayNightState.Night);
    }

    private void OnDestroy()
    {
        if (_timeManager != null)
        {
            _timeManager.OnDayStart -= () => HandleChangeDayNightState(TimeManager.DayNightState.Day);
            _timeManager.OnNightStart -= () => HandleChangeDayNightState(TimeManager.DayNightState.Night);
        }
    }

    public void HandleChangeDayNightState(TimeManager.DayNightState state)
    {
        if (state == TimeManager.DayNightState.Day)
        {
            _sunTransform.gameObject.SetActive(true);
            _moonTransform.gameObject.SetActive(false);
            _sunTransform.position = new Vector2(-_moonSunDistFromCenter, 0f); // Start sun on the left side
        }
        else
        {
            _sunTransform.gameObject.SetActive(false);
            _moonTransform.gameObject.SetActive(true);
            _moonTransform.position = new Vector2(-_moonSunDistFromCenter, 0f); // Start moon on the left side
        }
    }

    private void Update()
    {
        float gameTime = _timeManager.GetCurrentTime();
        float totalDuration = _timeManager.CurrentState == TimeManager.DayNightState.Day ? _timeManager.DayDurationInSeconds : _timeManager.NightDurationInSeconds;

        float ratioToComplete = gameTime / totalDuration;

        Transform activeTransform = _timeManager.CurrentState == TimeManager.DayNightState.Day ? _sunTransform : _moonTransform;
        RotateVisuals(activeTransform, ratioToComplete);
    }

    private void RotateVisuals(Transform activeTransform, float ratioToComplete)
    {
        Vector2 center = Vector2.zero;
        float radians = Mathf.Lerp(0, Mathf.PI, ratioToComplete);

        activeTransform.position = new Vector2(center.x + _moonSunDistFromCenter * -Mathf.Cos(radians),
                                               center.y + _moonSunDistFromCenter * Mathf.Sin(radians));
    }
}
