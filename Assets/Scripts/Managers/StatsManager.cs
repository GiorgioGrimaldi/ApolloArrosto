using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    /*
     * Non muori o muori solo 1 volta:
     *  Se rimane il 25%: 3 stelle
     *  Altrimenti: 2 stella
     *  
     * Muori 2 o 3 volte:
     *  Se rimane il 50%: 3 stelle
     *  Se rimane il 25%: 2 stelle
     *  Altrimenti: 1 stella
     * 
     * Muori 4 o più volte:
     *  Se rimane il 80%: 3 stelle
     *  Se rimane il 40%: 2 stelle
     *  Se rimane il 20%: 1 stelle
     *  Altrimenti: 0 stella
     *  
     *  (percentuali calcolate sull'ultimo terzo di ossigeno)
     */

    [SerializeField] private Image StarsFill;

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
        if(newState == GameState.LevelCompleted)
        {
            int stars = 0;
            int deaths = GameManager.Instance.Deaths;
            float oxygen = Mathf.Min(((OxygenManager.Instance.MaxOxygenDuration - OxygenManager.Instance.oxygenDuration) / (OxygenManager.Instance.MaxOxygenDuration/2)),1);
            
            if (deaths < 2)
            {
                if (oxygen > 0.25f)
                {
                    stars = 3;
                }
                else
                {
                    stars = 2;
                }
            }
            else if (deaths < 4)
            {
                if (oxygen > 0.5f)
                {
                    stars = 3;
                }
                else if (oxygen > 0.25f)
                {
                    stars = 2;
                }
                else
                {
                    stars = 1;
                }
            }
            else
            {
                if (oxygen > 0.5f)
                {
                    stars = 2;
                }
                else if (oxygen > 0.25f)
                {
                    stars = 1;
                }
                else
                {
                    stars = 0;
                }
            }

            StarsFill.fillAmount = stars/3.0f;
        }
    }
}
