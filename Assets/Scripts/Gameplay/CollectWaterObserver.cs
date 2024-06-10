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

        RootSegment[] rootToLoop = RootsConntectedToResouces.ToArray();
        foreach (RootSegment root in rootToLoop)
        {

            WaterResource[] waterResources = root.LinkedResources.ToArray();
            foreach (WaterResource waterResource in waterResources)
            {
                gameloopManager.CollectWater();
                bool isEmpty = waterResource.DissovleWater(waterToDisolve);

                if (isEmpty)
                {
                    root.LinkedResources.Remove(waterResource);
                }
            }

            RemoveRootWithoutResources();

            root.RemoveResourcesIfOutOfRange();
        }
    }

    private void RemoveRootWithoutResources()
    {
        for (int i = 0; i < RootsConntectedToResouces.Count; i++)
        {
            if (RootsConntectedToResouces[i].LinkedResources.Count == 0)
            {
                RootsConntectedToResouces.RemoveAt(i);
            }
        }
    }



}



