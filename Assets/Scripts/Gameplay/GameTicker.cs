using System;
using UnityEngine;

public class GameTicker : MonoBehaviour
{
    private int _tickRatePerSecond = 1;
    private float _timeTillNextTick;

    public event Action OnTick;

    void Start()
    {
        _timeTillNextTick = 1f / _tickRatePerSecond;
    }

    void Update()
    {
        _timeTillNextTick -= Time.deltaTime;

        if (_timeTillNextTick <= 0)
        {
            InvokeTick();
        }
    }

    private void InvokeTick()
    {
        OnTick?.Invoke();
        _timeTillNextTick = 1f / _tickRatePerSecond;
    }
}
