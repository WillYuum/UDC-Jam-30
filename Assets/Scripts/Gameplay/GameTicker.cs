using System;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
    [SerializeField] private int _tickRatePerSecond = 1;
    [HideInInspector] public float GameTime { get; private set; }

    [field: SerializeField] public float TimeMultiplier { get; private set; } = 1.5f; // Default to 1.5x

    public event Action OnTick;

    private float _normalTickDuration;
    private float _fastTickDuration;
    private float _timeTillNextTick;

    public bool IsFastMode { get; private set; } = false;

    public float TickDuration => IsFastMode ? _fastTickDuration : _normalTickDuration;

    void Start()
    {
        _normalTickDuration = 1f / _tickRatePerSecond;
        _fastTickDuration = _normalTickDuration / TimeMultiplier;
        _timeTillNextTick = _normalTickDuration;

        GameTime = 0f;
    }

    void Update()
    {
        HandleModeSwitch();

        _timeTillNextTick -= Time.deltaTime;

        if (_timeTillNextTick <= 0)
        {
            InvokeTick();
        }

        GameTime += IsFastMode ? Time.deltaTime * TimeMultiplier : Time.deltaTime;
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKey(KeyCode.E))
        {
            SetFastMode(true, 1.5f);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            SetFastMode(true, 2f);
        }
        else if (Input.GetKey(KeyCode.T))
        {
            SetFastMode(true, 2.5f);
        }
        else
        {
            SetFastMode(false, 1f);
        }
    }

    private void SetFastMode(bool isFast, float multiplier)
    {
        IsFastMode = isFast;
        TimeMultiplier = multiplier;
        _fastTickDuration = _normalTickDuration / TimeMultiplier;
        _timeTillNextTick = _fastTickDuration;
    }

    private void InvokeTick()
    {
        OnTick?.Invoke();
        _timeTillNextTick = IsFastMode ? _fastTickDuration : _normalTickDuration;
    }

    public void ToggleTicker(bool active)
    {
        enabled = active;
    }
}
