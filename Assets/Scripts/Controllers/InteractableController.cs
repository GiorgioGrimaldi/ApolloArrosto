using UnityEngine;
using UnityEngine.UI;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private bool IsNormallyActiveOnOrangeTurn;
    [SerializeField] private ActivatableController LinkedObject;
    [SerializeField] private LayerMask AstronautLayerMask;
    [SerializeField] private Image CooldownImage;
    [SerializeField] private Animator InteractableAnimator;
    [SerializeField] private AudioClip PressedAudioClip;
    [SerializeField] private AudioClip ReleasedAudioClip;
    [SerializeField] private AudioSource AudioSource;

    private float cooldownDuration = 1.5f;
    private float cooldownTime;
    public bool IsCoolingDown => cooldownTime < cooldownDuration;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (AstronautLayerMask.Includes(other.gameObject.layer))
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController.IsOrangeAstronaut && characterController.interactable == null)
            {
                characterController.interactable = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (AstronautLayerMask.Includes(other.gameObject.layer))
        {
            CharacterController characterController = other.gameObject.GetComponent<CharacterController>();
            if (characterController.IsOrangeAstronaut && characterController.interactable == this)
            {
                characterController.interactable = null;
            }
        }
    }

    private void Awake()
    {
        cooldownTime = cooldownDuration;
        CooldownImage.enabled = IsCoolingDown;

        if (LinkedObject != null)
        {
            LinkedObject.enabled = !IsNormallyActiveOnOrangeTurn;
            InteractableAnimator.SetBool(Utility.IsActive_ParameterId, false);
        }
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    
    private void Update()
    {
        CooldownImage.fillAmount = Mathf.Max(1 - cooldownTime / cooldownDuration, 0);
        CooldownImage.enabled = IsCoolingDown;
    }

    private void FixedUpdate()
    {
        if (IsCoolingDown && GameManager.Instance.State.IsPlayable())
        {
            cooldownTime += Time.fixedDeltaTime;
            
            if (!IsCoolingDown && LinkedObject != null)
            {
                LinkedObject.enabled = IsNormallyActiveOnOrangeTurn;
                InteractableAnimator.SetBool(Utility.IsActive_ParameterId, true);
                AudioSource.PlayOneShot(ReleasedAudioClip);
            }
        }
    }
    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (newState is GameState.BlueAstronautNotStarted or GameState.OrangeAstronautNotStarted)
        {
            LinkedObject.enabled = newState == GameState.OrangeAstronautNotStarted ? IsNormallyActiveOnOrangeTurn : !IsNormallyActiveOnOrangeTurn;
            InteractableAnimator.SetBool(Utility.IsActive_ParameterId, newState == GameState.OrangeAstronautNotStarted);
            cooldownTime = cooldownDuration;
        }
    }

    public void TryInteract()
    {
        if (!IsCoolingDown && LinkedObject != null)
        {
            cooldownTime = 0;
            CooldownImage.fillAmount = 1;
            LinkedObject.enabled = !IsNormallyActiveOnOrangeTurn;
            InteractableAnimator.SetBool(Utility.IsActive_ParameterId, false);
            AudioSource.PlayOneShot(PressedAudioClip);
        }
    }
}
