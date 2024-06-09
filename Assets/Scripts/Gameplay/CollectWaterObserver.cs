using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class CollectWaterObserver : MonoBehaviourSingleton<CollectWaterObserver>
{

    [HideInInspector] public List<RootSegment> RootsConntectedToResouces { get; private set; } = new List<RootSegment>();

    private GameloopManager gameloopManager;

    private void Awake()
    {
        GameTicker gameTicker = FindObjectOfType<GameTicker>();
        gameTicker.OnTick += HandleAbsorbWater;
        gameloopManager = FindObjectOfType<GameloopManager>();
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

        float waterToDisolve = gameloopManager.TreeStats.WaterAbsorbtionRate.Value;

        foreach (RootSegment root in RootsConntectedToResouces)
        {
            WaterResource[] waterResources;
            if (root.IsWaterColliding(out waterResources))
            {
                foreach (WaterResource waterResource in waterResources)
                {
                    gameloopManager.CollectWater();
                    waterResource.DissovleWater(waterToDisolve);
                }
            }


            root.RemoveResourcesIfOutOfRange();
        }
    }



}



