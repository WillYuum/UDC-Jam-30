using UnityEngine;

// [RequireComponent(typeof(RootSegment))]
public class RootGrowth : MonoBehaviour
{
    private RootSegment _rootSegment;

    private float _timePassed = 0f;

    public float TimeMaxGrowth = 2.0f;


    private void Awake()
    {
        _rootSegment = GetComponent<RootSegment>();
        TimeMaxGrowth = FindObjectOfType<GameloopManager>().TreeStats.RootGrowthDuration;
    }





    public void Grow(float growRate)
    {
        _timePassed += growRate;

        Vector2 startPosition = _rootSegment.GetStartPosition();
        _rootSegment.UpdateEndPosition(Vector2.Lerp(startPosition, _rootSegment.ActualEndPosition, _timePassed / TimeMaxGrowth));
    }

    public bool IsFullyGrown()
    {
        return _timePassed >= TimeMaxGrowth;
    }
}
