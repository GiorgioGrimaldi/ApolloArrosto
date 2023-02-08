using TMPro;
using UnityEngine;

public class DeathCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text DeathCounterText;

    private void Update()
    {
        DeathCounterText.text = "Tentativi: " + GameManager.Instance.Deaths.ToString();
    }
}
