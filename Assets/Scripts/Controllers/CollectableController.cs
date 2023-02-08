using UnityEngine;

public class CollectableController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Collider2D Collider2D;
    [SerializeField] private CollectablesManager CollectablesManager;
    [SerializeField] private LayerMask AstronautLayerMask;
    [SerializeField] private AudioClip CollectableAudioClip;
    [SerializeField] private AudioSource AudioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (AstronautLayerMask.Includes(other.gameObject.layer))
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (!characterController.IsOrangeAstronaut)
            {
                CollectablesManager.Collect();
                AudioSource.PlayOneShot(CollectableAudioClip);
                Collider2D.enabled = false;
                SpriteRenderer.enabled = false;
            }
        }
    }

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
        if (newState.IsAstronautNotStarted() && previousState != GameState.Pause)
        {
            Collider2D.enabled = true;
            SpriteRenderer.enabled = true;
        }
    }

}
