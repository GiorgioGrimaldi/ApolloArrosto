using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PathFollowerController : MonoBehaviour
{
    [SerializeField] private List<Transform> PathWaypoints = new List<Transform>();
    [SerializeField] private float CooldownDuration = 1.5f;
    [SerializeField] private float Speed = 1f;

    private bool isMovingBackwards;
    private int currentWaypointIndex;

    private float cooldownTime;
    public bool IsCoolingDown => cooldownTime < CooldownDuration;

    private void Awake()
    {
        cooldownTime = CooldownDuration;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        currentWaypointIndex = 1;
        isMovingBackwards = false;
        if (PathWaypoints.Count > 0)
        {
            transform.position = PathWaypoints[0].position;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (newState.IsPlayable() && previousState != GameState.Pause)
        {
            currentWaypointIndex = 1;
            isMovingBackwards = false;
            if (PathWaypoints.Count > 0)
            {
                transform.position = PathWaypoints[0].position;
            }
        }
    }

    private void FixedUpdate()
    {
        if (PathWaypoints.Count < 2 || !GameManager.Instance.State.IsAstronautStarted())
        {
            return;
        }

        if (IsCoolingDown)
        {
            cooldownTime += Time.fixedDeltaTime;

            if (IsCoolingDown)
            {
                return;
            }
        }

        if (Vector2.Distance(PathWaypoints[currentWaypointIndex].position, transform.position) < .01f)
        {
            cooldownTime = 0;
            if (currentWaypointIndex == 0 || currentWaypointIndex == PathWaypoints.Count - 1)
            {
                isMovingBackwards = !isMovingBackwards;
            }
            currentWaypointIndex = isMovingBackwards ? currentWaypointIndex - 1 : currentWaypointIndex + 1;
        }


         transform.position = Vector2.MoveTowards(transform.position, PathWaypoints[currentWaypointIndex].position, Time.fixedDeltaTime * Speed);
    }
}
