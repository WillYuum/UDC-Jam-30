using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class CollectWaterObserver : MonoBehaviourSingleton<CollectWaterObserver>
{

    [HideInInspector] public List<RootSegment> RootsConntectedToResouces { get; private set; }

    private GameloopManager gameloopManager;

    private void Awake()
    {
        RootsConntectedToResouces = new List<RootSegment>();

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
                if (waterResource == null)
                {
                    root.LinkedResources.Remove(waterResource);
                    continue;
                }


                bool hasCollectedWater = gameloopManager.CollectWater();

                if (hasCollectedWater == false)
                {
                    return;
                }

                bool isEmpty = waterResource.DissovleWater(waterToDisolve);

                if (isEmpty)
                {
                    root.LinkedResources.Remove(waterResource);
                }
            }

            RemoveRootWithoutResources(root);

            root.RemoveResourcesIfOutOfRange();
        }
    }

    private void RemoveRootWithoutResources(RootSegment rootSegment)
    {
        if (rootSegment.LinkedResources.Count == 0)
        {
            RootsConntectedToResouces.Remove(rootSegment);
        }
    }



}



