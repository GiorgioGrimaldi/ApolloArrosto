using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public enum CharacterType
{
    Blue_Astronaut,
    Orange_Astronaut,
    Ghost_Astronaut,
}

public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterType Character;
    [SerializeField] private Transform GroundCollider;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private LayerMask DeathLayerMask;

    [SerializeField] private Animator Animator;
    [SerializeField] private Rigidbody2D Rigidbody2D;
    [SerializeField] private Collider2D Collider2D;

    [SerializeField] private AudioClip JumpAudioClip;
    [SerializeField] private AudioSource JumpAudioSource;

    [SerializeField] private AudioClip JetpackAudioClip;
    [SerializeField] private AudioSource JetpackAudioSource;
    
    public bool IsOrangeAstronaut => (Character == CharacterType.Orange_Astronaut);
    public bool IsGrounded { get; private set; }
    public bool IsFacingRight { get; private set; }

    private Vector3 smoothingVelocity = Vector3.zero;

    private float jumpTimeCounter;
    private float jumpTime = 0.17f;
    private float jumpForce = 7.5f;
    private bool canJump = true;
    private bool isJumping = false;

    private float jetForce = 7f;
    private float movementSmoothing = .05f;
    private float groundedRadius = 0.3f;

    public InteractableController interactable;

    private void OnCollisionEnter2D(Collision2D other)
    {
        int layer = other.gameObject.layer;

        if (DeathLayerMask.Includes(layer) && GameManager.Instance.State.IsAstronautStarted())
        {
            GameManager.Instance.Die();
        }
    }

    private void Awake()
    {
        bool isPlayable = GameManager.Instance.State.IsPlayable();
        Rigidbody2D.simulated = isPlayable;
        Collider2D.enabled = isPlayable;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        bool isPlayable = newState.IsPlayable();
        Rigidbody2D.simulated = isPlayable;
        Collider2D.enabled = isPlayable;
        Animator.speed = isPlayable ? 1 : 0;
    }

    public void ProcessInput_FixedUpdate(InputState inputState)
    {
        if (GameManager.Instance.State is GameState.BlueAstronautNotStarted or GameState.OrangeAstronautNotStarted)
        {
            if (inputState.Horizontal != 0 || inputState.Jump)
            {
                GameManager.Instance.UpdateGameState((GameManager.Instance.State == GameState.BlueAstronautNotStarted) ? GameState.BlueAstronautStarted : GameState.OrangeAstronautStarted);
            }
        }

        Collider2D collider = Physics2D.OverlapCircle(GroundCollider.position, groundedRadius, GroundLayerMask);
        IsGrounded = collider != null;

        Interact(inputState.Interact);
        Move(inputState);
        UpdateAnimator();
    }

    private void Interact(bool interact)
    {
        if(interactable != null && interact)
        {
            interactable.TryInteract();
        }
    }
    
    private void Move(InputState inputState)
    {
        float horizontal = inputState.Horizontal;
        bool jump = inputState.Jump;

        Vector3 targetVelocity = new Vector2(horizontal * 10f, Rigidbody2D.velocity.y);
        Rigidbody2D.velocity = Vector3.SmoothDamp(Rigidbody2D.velocity, targetVelocity, ref smoothingVelocity, movementSmoothing);

        if (horizontal != 0)
        {
            bool hasToFaceRight = horizontal > 0;
            if (hasToFaceRight != IsFacingRight)
            {
                Flip();
            }
        }

        if (IsOrangeAstronaut)
        {
            if (jump)
            {
                if(canJump && IsGrounded)
                {
                    
                    Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, jumpForce);
                    JumpAudioSource.PlayOneShot(JumpAudioClip);
                    
                    IsGrounded = false;
                }

                if (!IsGrounded)
                {
                    Rigidbody2D.AddForce(new Vector2(0f, jetForce));
                    if (!JetpackAudioSource.isPlaying)
                    {
                        JetpackAudioSource.clip = JetpackAudioClip;
                        JetpackAudioSource.Play();
                    }
                }
                canJump = false;
            }
            else
            {
                canJump = true;
                JetpackAudioSource.Stop();
            }
        }
        else
        {
            if (jump)
            {
                if (canJump && IsGrounded)
                {
                    canJump = false;
                    isJumping = true;
                    IsGrounded = false;

                    jumpTimeCounter = jumpTime;

                    Rigidbody2D.velocity = (new Vector2(Rigidbody2D.velocity.x, jumpForce));
                    if (GameManager.Instance.State == GameState.BlueAstronautStarted)
                    {
                        JumpAudioSource.PlayOneShot(JumpAudioClip);
                    }
                }

                if (isJumping)
                {
                    if (jumpTimeCounter > 0)
                    {
                        Rigidbody2D.velocity = (new Vector2(Rigidbody2D.velocity.x, jumpForce));
                        jumpTimeCounter -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        isJumping = false;
                    }
                }
            }
            else
            {
                jumpTimeCounter = 0;

                if (IsGrounded)
                {
                    canJump = true;
                    isJumping = false;
                }
            }
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    
    private void UpdateAnimator()
    {
        Animator.SetBool(Utility.IsGrounded_ParameterId, IsGrounded);

        Animator.SetFloat(Utility.AbsoluteHorizontalSpeed_ParameterId, Mathf.Abs(Rigidbody2D.velocity.x));
        Animator.SetFloat(Utility.VerticalSpeed_ParameterId, Rigidbody2D.velocity.y);

        if (IsOrangeAstronaut)
        {
            Animator.SetFloat(Utility.JetPack_ParameterId, JetpackAudioSource.isPlaying ? 1 : 0);
        }
    }
}
