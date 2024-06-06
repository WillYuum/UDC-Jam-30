using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class CollectWaterObserver : MonoBehaviourSingleton<CollectWaterObserver>
{

    [HideInInspector] public List<RootSegment> RootsConntectedToResouces { get; private set; } = new List<RootSegment>();

    private void Awake()
    {
        GameTicker gameTicker = FindObjectOfType<GameTicker>();
        gameTicker.OnTick += HandleAbsorbWater;
    }

    void Start()
    {
        // AvailableWaterResources = new List<WaterResource>(FindObjectsOfType<WaterResource>(false));
    }


    public void ObserveRoot(RootSegment rootSegment)
    {
        RootsConntectedToResouces.Add(rootSegment);
    }


    private void HandleAbsorbWater()
    {
        if (RootsConntectedToResouces.Count == 0)
        {
            return;
        }

        foreach (var root in RootsConntectedToResouces)
        {
            WaterResource[] waterResources;
            if (root.IsWaterColliding(out waterResources))
            {
                foreach (var waterResource in waterResources)
                {
                    waterResource.DissovleWater(1);
                }
            }
        }

        CheckIfStillConnectToResouce();
    }


    private void CheckIfStillConnectToResouce()
    {
        foreach (var root in RootsConntectedToResouces)
        {
            if (!root.IsWaterColliding())
            {
                RootsConntectedToResouces.Remove(root);
            }
        }
    }


}



