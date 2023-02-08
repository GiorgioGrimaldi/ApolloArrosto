using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    [SerializeField] private int NeededCollectablesCount = 3;
    [SerializeField] private DoorController DoorController;

    private int collectableCount = 0;
    
    private void Awake()
    {
        if (collectableCount >= NeededCollectablesCount)
        {
            DoorController.Open();
        }
        else
        {
            DoorController.Close();
        }
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (newState.IsAstronautNotStarted() && previousState != GameState.Pause)
        {
            collectableCount = 0;
            if (collectableCount >= NeededCollectablesCount)
            {
                DoorController.Open();
            }
            else
            {
                DoorController.Close();
            }
        }
    }

    public void Collect()
    {
        collectableCount++;
        if (collectableCount >= NeededCollectablesCount)
        {
            DoorController.Open();
        }
    }
}
