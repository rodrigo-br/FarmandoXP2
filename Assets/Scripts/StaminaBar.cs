using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    private Stamina stamina;

    private void Awake()
    {
        stamina = new Stamina(100);
    }

    private void OnEnable()
    {
        Stamina.OnStaminaChange += UpdateUI;
    }

    private void OnDisable()
    {
        Stamina.OnStaminaChange -= UpdateUI;
    }

    private void UpdateUI()
    {
        staminaBar.transform.localScale = new Vector3((float)stamina.GetCurrentStamina() / 100f, 1, 1);
    }
}
