using System;
using System.Collections.Generic;
using UnityEngine;
public class RewindManager : MonoBehaviour
{
    public static RewindManager Instance;

    [SerializeField] private CharacterController OrangeAstronautPrefab;
    private CharacterController orangeAstronaut;

    [SerializeField] private CharacterController BlueAstronautPrefab;
    private CharacterController blueAstronaut;

    [SerializeField] private CharacterController GhostAstronautPrefab;
    private CharacterController ghostAstronaut;

    private List<InputState> recordedBlueAstronautInputs;
    private int blueIndex;
    
    private List<Vector3> recordedOrangeAstronautPositions;
    private int orangeIndex;

    private int rewindingSpeed;
    private int parameterReload = 40;

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
    private void FixedUpdate()
    {
        if (GameManager.Instance.State == GameState.BlueAstronautStarted)
        {
            recordedBlueAstronautInputs.Add(InputManager.Instance.CurrentInputState);
        }
        else if(GameManager.Instance.State == GameState.OrangeAstronautStarted)
        {
            recordedOrangeAstronautPositions.Add(orangeAstronaut.transform.position);
            MoveGhost();
        }
        else if (GameManager.Instance.State == GameState.Rewinding)
        {
            Rewind();
        }
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if(previousState == GameState.Pause)
        {
            return;    
        }
        
        if (newState == GameState.BlueAstronautNotStarted)
        {
            DestroyAllAstronaut();
            InstantiateBlueAstronaut();
            InputManager.Instance.CharacterController = blueAstronaut;
        }
        else if (newState == GameState.BlueAstronautStarted)
        {
            recordedBlueAstronautInputs = new List<InputState>();
            blueIndex = 0;
        }
        else if (newState == GameState.OrangeAstronautNotStarted)
        {
            DestroyAllAstronaut();
            InstantiateGhostAstronaut();
            InstantiateOrangeAstronaut();
            InputManager.Instance.CharacterController = orangeAstronaut;
        }
        else if (newState == GameState.OrangeAstronautStarted)
        {
            recordedOrangeAstronautPositions = new List<Vector3>();
            blueIndex = 0;
            orangeIndex = 0;
        }
        else if (newState == GameState.Rewinding)
        {
            DestroyGhostAstronaut();
            orangeIndex = recordedOrangeAstronautPositions.Count - 1;
            rewindingSpeed = orangeIndex / parameterReload;
        }
        else if(newState == GameState.LevelCompleted)
        {
            DestroyGhostAstronaut();
        }
    }

    private void Rewind()
    {
        if (rewindingSpeed > 0 && orangeIndex - rewindingSpeed > 0)
        {
            orangeIndex = Math.Max(0, orangeIndex - rewindingSpeed);
        }
        else if (orangeIndex - 1 > 0)
        {
            orangeIndex--;
        }
        else
        {
            orangeIndex = 0;
            orangeAstronaut.transform.position = recordedOrangeAstronautPositions[0];
            GameManager.Instance.UpdateGameState(GameState.OrangeAstronautNotStarted);
            return;
        }
        orangeAstronaut.transform.position = recordedOrangeAstronautPositions[orangeIndex];
    }
    private void MoveGhost()
    {
        if (blueIndex < recordedBlueAstronautInputs.Count)
        {
            ghostAstronaut.ProcessInput_FixedUpdate(recordedBlueAstronautInputs[blueIndex]);
        }
        else if(blueIndex > recordedBlueAstronautInputs.Count)
        {
            GameManager.Instance.Die();
        }
        
        blueIndex++;
    }

    private void DestroyAllAstronaut()
    {
        DestroyBlueAstronaut();
        DestroyOrangeAstronaut();
        DestroyGhostAstronaut();
    }

    private void DestroyBlueAstronaut()
    {
        if (blueAstronaut != null)
        {
            Destroy(blueAstronaut.gameObject);
            blueAstronaut = null;
        }
    }
    private void DestroyOrangeAstronaut()
    {
        if (orangeAstronaut != null)
        {
            Destroy(orangeAstronaut.gameObject);
            orangeAstronaut = null;
        }
        AstronautsPlaceholdersReferences.Instance.OrangeStartPlaceholder.gameObject.SetActive(true);
    }
    private void DestroyGhostAstronaut()
    {
        if (ghostAstronaut != null)
        {
            Destroy(ghostAstronaut.gameObject);
            ghostAstronaut = null;
        }
    }


    private void InstantiateBlueAstronaut()
    {
        if (blueAstronaut == null)
        {
            blueAstronaut = Instantiate(BlueAstronautPrefab, AstronautsPlaceholdersReferences.Instance.BlueStartPlaceholder.position, Quaternion.identity);
        }
        AstronautsPlaceholdersReferences.Instance.OrangeStartPlaceholder.gameObject.SetActive(true);
    }
    private void InstantiateOrangeAstronaut()
    {
        if (orangeAstronaut == null)
        {
            orangeAstronaut = Instantiate(OrangeAstronautPrefab, AstronautsPlaceholdersReferences.Instance.OrangeStartPlaceholder.position, Quaternion.identity);
        }
        AstronautsPlaceholdersReferences.Instance.OrangeStartPlaceholder.gameObject.SetActive(false);
    }
    private void InstantiateGhostAstronaut()
    {
        if (ghostAstronaut == null)
        {
            ghostAstronaut = Instantiate(GhostAstronautPrefab, AstronautsPlaceholdersReferences.Instance.BlueStartPlaceholder.position, Quaternion.identity);
        }
    }
}