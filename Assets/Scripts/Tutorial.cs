using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject TutorialText1;
    [SerializeField] private GameObject TutorialText2;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (newState is GameState.BlueAstronautNotStarted or GameState.BlueAstronautStarted)
        {
            if (TutorialText1 != null)
            {
                TutorialText1?.SetActive(true);
            }
            if (TutorialText2 != null)
            {
                TutorialText2?.SetActive(false);
            }
        }
        else if (newState is GameState.OrangeAstronautNotStarted or GameState.OrangeAstronautStarted)
        {
            if (TutorialText1 != null)
            {
                TutorialText1?.SetActive(false);
            }
            if (TutorialText2 != null)
            {
                TutorialText2?.SetActive(true);
            }
        }
    }
}
