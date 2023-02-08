using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public CharacterController CharacterController;

    public InputState CurrentInputState { get; private set; }

    private PlayerInputActions actions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
            CurrentInputState = new InputState();
            actions = new PlayerInputActions();
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

    private void Update()
    {
        bool pause = actions.Pause.Pause.WasPressedThisFrame();
        if (pause)
        {
            GameManager.Instance.TogglePause();
            return;
        }
    }

    private void FixedUpdate()
    {
        if (CharacterController == null)
        {
            return;
        }

        float x = actions.Astronaut.Move.ReadValue<Vector2>().x;
        bool jump = actions.Astronaut.Jump.IsPressed();
        bool interact = actions.Astronaut.Interact.IsPressed();
        
        CurrentInputState = new InputState(x, jump, interact);
        CharacterController.ProcessInput_FixedUpdate(CurrentInputState);
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (newState is GameState.Pause or GameState.Rewinding or GameState.LevelCompleted)
        {
            actions.Astronaut.Disable();
        }
        else
        {
            actions.Astronaut.Enable();
        }

        if (newState != GameState.None && newState != GameState.MainMenu)
        {
            actions.Pause.Enable();
        }
        else
        {
            actions.Pause.Disable();
        }
    }
}
