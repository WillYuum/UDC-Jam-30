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
        float ratioToComplete = _timeManager.CurrentState == TimeManager.DayNightState.Day ? _timeManager.GetDayTimeRatio() : _timeManager.GetNightTimeRatio();
        Transform activeTransform = _timeManager.GetCurrentState() == TimeManager.DayNightState.Day ? _sunTransform : _moonTransform;
        RotateVisuals(activeTransform, ratioToComplete);
    }

    // private float GetTimeRatio()
    // {
    // float currentTime = _timeManager.GetCurrentTime();
    // float totalCycleDuration = TimeManager.TotalCycleDuration; // Duration of a full day-night cycle
    // return currentTime / totalCycleDuration;
    // }

    private void RotateVisuals(Transform activeTransform, float ratioToComplete)
    {
        Vector2 center = Vector2.zero;
        float angle = ratioToComplete * Mathf.PI; // 180 degrees (π radians) scaled by the ratio

        // Rotate clockwise from left to right
        activeTransform.position = new Vector2(center.x + _moonSunDistFromCenter * -Mathf.Cos(angle),
                                               center.y + _moonSunDistFromCenter * Mathf.Sin(angle));
    }
}
