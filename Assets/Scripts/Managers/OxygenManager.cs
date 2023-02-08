using UnityEngine;
using UnityEngine.UI;

public class OxygenManager : MonoBehaviour
{
    public static OxygenManager Instance;

    [SerializeField] private Image OxygenFill;
    [SerializeField] public float MaxOxygenDuration = 10f;

    public float oxygenTime;
    public float oxygenDuration;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        oxygenDuration = MaxOxygenDuration;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState newState, GameState previousState)
    {
        if (previousState != GameState.Pause)
        {
            if (newState == GameState.BlueAstronautNotStarted)
            {
                oxygenDuration = MaxOxygenDuration;
                oxygenTime = 0;
            }
            else if (newState == GameState.OrangeAstronautNotStarted)
            {
                if(previousState == GameState.BlueAstronautStarted)
                {
                    oxygenDuration = oxygenTime;
                }
                oxygenTime = 0;
            }
        }
    }

    private void Update()
    {
        OxygenFill.fillAmount = 1 - (oxygenTime / oxygenDuration);
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.State.IsAstronautStarted())
        {
            return;
        }

        if (oxygenTime < oxygenDuration)
        {
            oxygenTime += Time.fixedDeltaTime;

            if (oxygenTime >= oxygenDuration && GameManager.Instance.State == GameState.BlueAstronautStarted)
            {
                GameManager.Instance.Die();
            }
        }
    }
}
