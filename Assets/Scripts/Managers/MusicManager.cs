using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioClip MainMenuAudioClip;
    [SerializeField] private AudioClip LevelAudioClip;
    [SerializeField] private AudioSource AudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        AudioClip audioClip = null;
        
        if (newState == GameState.MainMenu)
        {
            audioClip = MainMenuAudioClip;
        }
        else if (newState != GameState.None)
        {
            audioClip = LevelAudioClip;
        }

        if (audioClip != null && audioClip != AudioSource.clip)
        {
            AudioSource.clip = audioClip;
            AudioSource.Play();
        }
    }
}
