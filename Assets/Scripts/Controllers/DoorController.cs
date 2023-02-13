using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator DoorAnimator;
    [SerializeField] private LayerMask AstronautLayerMask;
    [SerializeField] private AudioClip DoorAudioClip;
    [SerializeField] private AudioSource AudioSource;

    public bool IsOpen { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOpen && AstronautLayerMask.Includes(other.gameObject.layer))
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (!characterController.IsOrangeAstronaut)
            {
                if (GameManager.Instance.State == GameState.BlueAstronautStarted)
                {
                    GameManager.Instance.UpdateGameState(GameState.OrangeAstronautNotStarted);
                }
                else if (GameManager.Instance.State == GameState.OrangeAstronautStarted)
                {
                    GameManager.Instance.UpdateGameState(GameState.LevelCompleted);
                }
            }
        }
    }

    [ContextMenu("Open")]
    public void Open()
    {
        if (!IsOpen && !AudioSource.isPlaying)
        {
            AudioSource.PlayOneShot(DoorAudioClip);
        }
        IsOpen = true;
        DoorAnimator.SetBool(Utility.IsOpen_ParameterId, true);
    }

    [ContextMenu("Close")]
    public void Close()
    {
        IsOpen = false;
        DoorAnimator.SetBool(Utility.IsOpen_ParameterId, false);
    }
}
