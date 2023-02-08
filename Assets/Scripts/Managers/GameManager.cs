using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State = GameState.MainMenu;
    public GameState PreviousState = GameState.MainMenu;

    public static event Action<GameState, GameState> OnGameStateChanged;
    [SerializeField] private AudioClip DeathAudioClip;
    [SerializeField] private AudioSource AudioSource;

    public int Deaths { get; private set; }

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
    }

    public void UpdateGameState(GameState newState)
    {
        if (State == newState)
        {
            return;
        }

        PreviousState = State;
        //State = GameState.None;
        //OnGameStateChanged?.Invoke(newState);
        State = newState;

        if (newState == GameState.BlueAstronautNotStarted && !PreviousState.IsPlayable() && PreviousState != GameState.Pause && PreviousState != GameState.Rewinding)
        {
            Deaths = 0;
        }

        OnGameStateChanged?.Invoke(newState, PreviousState);
    }

    public void TogglePause()
    {
        if (State == GameState.Pause)
        {
            UpdateGameState(PreviousState);
        }
        else if (State.IsPlayable())
        {
            UpdateGameState(GameState.Pause);
        }
    }

    public void Die(bool forceBlueAstronautTurn = false)
    {
        if (State != GameState.Rewinding)
        {
            AudioSource.PlayOneShot(DeathAudioClip);
            Deaths++;
            if (State == GameState.LevelCompleted)
            {
                Deaths = 0;
            }
            
            if (forceBlueAstronautTurn || State == GameState.BlueAstronautStarted)
            {
                State = GameState.Rewinding;
                UpdateGameState(GameState.BlueAstronautNotStarted);
            }
            else if (State == GameState.OrangeAstronautStarted)
            {
                UpdateGameState(GameState.Rewinding);
            }
        }
    }
}

public enum GameState
{
    None,
    MainMenu,
    Pause,
    BlueAstronautNotStarted,
    BlueAstronautStarted,
    OrangeAstronautNotStarted,
    OrangeAstronautStarted,
    Rewinding,
    LevelCompleted,
}



