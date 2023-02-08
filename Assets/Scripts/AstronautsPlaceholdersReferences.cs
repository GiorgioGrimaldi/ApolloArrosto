using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautsPlaceholdersReferences : MonoBehaviour
{
    public static AstronautsPlaceholdersReferences Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameManager.Instance.UpdateGameState(GameState.BlueAstronautNotStarted);
        }
        else
        {
            throw new System.Exception("There is more than one LevelItemsReferences in the scene");
        }
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    [SerializeField] public Transform BlueStartPlaceholder;
    [SerializeField] public Transform OrangeStartPlaceholder;
}
