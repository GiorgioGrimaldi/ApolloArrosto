using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private int currentLevel;
    public bool IsLastLevel => currentLevel == SceneManager.sceneCountInBuildSettings - 1;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public void PlayLevel(int level)
    {
        if (level >= SceneManager.sceneCountInBuildSettings)
        {
            return;
        }

        currentLevel = level;
        GameManager.Instance.UpdateGameState(GameState.None);
        SceneManager.LoadScene(currentLevel);
    }
    public void PlayMainMenu()
    {
        PlayLevel(0);
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
    }

    public void PlayFirstLevel() => PlayLevel(1);
    public void PlayNextLevel() => PlayLevel(currentLevel + 1);
    public void RestartLevel() => PlayLevel(currentLevel);
}




