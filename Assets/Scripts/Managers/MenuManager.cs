using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject LevelCompletedMenu;

    [SerializeField] private GameObject NextLevelButton;
    [SerializeField] private GameObject LevelCompletedText;
    [SerializeField] private GameObject DemoCompletedText;


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
        PauseMenu.SetActive(newState == GameState.Pause);
        MainMenu.SetActive(newState == GameState.MainMenu);
        LevelCompletedMenu.SetActive(newState == GameState.LevelCompleted);

        if(newState == GameState.LevelCompleted)
        {
            bool isLastLevel = LevelManager.Instance.IsLastLevel;
            NextLevelButton.SetActive(!isLastLevel);
            LevelCompletedText.SetActive(!isLastLevel);
            DemoCompletedText.SetActive(isLastLevel);
        }
    }

    public void PlayFirstLevel()
    {
        LevelManager.Instance.PlayFirstLevel();
    }
    public void ResumeLevel()
    {
        GameManager.Instance.TogglePause();
    }
    public void RestartLevel()
    {
        GameManager.Instance.Die(true);
    }
    public void PlayNextLevel()
    {
        LevelManager.Instance.PlayNextLevel();
    }
    public void PlayMainMenu()
    {
        LevelManager.Instance.PlayMainMenu();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}   
